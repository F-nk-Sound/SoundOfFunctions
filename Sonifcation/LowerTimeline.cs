
using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using System.Diagnostics;

namespace Sonification;

/// <summary>
/// Backend of the Timeline UI. This is the backbone of Audio Generation. 
/// </summary>
public partial class LowerTimeline : Node {
    
    /// <summary>
    /// Plays the Audio of the Timeline using <c> LowerTimeline.Sonify() </c>.
    /// </summary>
    private AudioStreamPlayer player;	

    /// <summary>
    /// List of functions on the Timeline.
    /// </summary>
    private List<Function> functions;	

    /// <summary>
    /// Timer to manage and synchronize audio playback within the Timeline.
    /// </summary>
    private TimeKeeper timer;           

    /// <summary>
    /// Current function being played by the Timeline.
    /// </summary>
    private int currFunction;       

    /// <summary>
    /// Runtime of the Timeline (in seconds).
    /// </summary>    
    private int RunTime {get; set;}				

    /// <summary>
    /// If <c> true </c>, Timeline audio playback is active.
    /// </summary>
    public bool IsPlaying {get; set;}	

    /// <summary>
    /// Current position of Timeline audio playback (in seconds).
    /// </summary>
    public double CurrPosition {get; set;}

    /// <summary>
    /// Initializes a new LowerTimeline Node.
    /// </summary>
    /// <param name="parent">The parent of this LowerTimeline node.</param>
    public LowerTimeline(Node parent) {
        RunTime = 0;
        currFunction = 0;
        CurrPosition = 0.0;
        IsPlaying = false;
        functions = new List<Function>();
        Name = "LowerTimeline";

        // Audio characteristics 
        timer = new TimeKeeper();
        player = new AudioStreamPlayer {
            Stream = new AudioStreamGenerator(),
            Name = "LowerTimelinePlayer"
        };

        // Scene Tree stuff
        AddChild(player);
        parent.AddChild(this);
        SetProcess(false);
    }

    /// <summary>
    /// Adds a Function Node to the end of the Timeline.
    /// </summary>
    /// <param name="func">Function to be added.</param>
    public void Add(Function func) {
        AddChild(func);	
        func.SetProcess(false);
        RunTime += func.RunTime;
        functions.Add(func);
    }

    /// <summary>
    /// Adds a list of Functions to the end of the Timeline.
    /// </summary>
    /// <param name="funcList">List of Functions to be added.</param>
    public void Add(List<Function> funcList) {
        foreach(Function f in funcList) Add(f);
    }
    
    /// <summary>
    /// Grabs the specified Function from the Timeline.
    /// </summary>
    /// <param name="index">Location of Function on the Timeline (0 indexed).</param>
    /// <returns>Function a the specified location.</returns>
    public Function GetFunction(int index) {
        if(index + 1 > functions.Count || index < 0) return null;
        return functions[index];
    }
    
    /// <summary>
    /// Removes the specified Function from the Timeline.
    /// </summary>
    /// <param name="index">Location of Function on the Timeline (0 indexed).</param>
    /// <returns><c>true</c> if removal was successful.</returns>
    public bool RemoveFunction(int index) {
        Function func = GetFunction(index);
        if(func == null) return false;
        functions.Remove(func);
        return true;
    }

    /// <summary>
    /// Plays the current function of the Timeline at the appropriate time.
    /// </summary>
    private void Play() {
        // Stop playback if necessary
		if(StopPlaying() || currFunction > functions.Count) return;

        // Grab the current timer position and the time to allow the next function to play
        int currTime = timer.ClockTimeRounded;
        int updateTime = (currFunction == 0) ? functions[currFunction].RunTime : functions[currFunction - 1].RunTime;

        // Play the functions within the timeline at the appropriate time
        if(currFunction == 0 || (int) timer.ElapsedTime == updateTime) {
            functions[currFunction].StartPlaying();
            timer.ResetTracking();
            currFunction++;
        }

        // Debugging Statments
        if(timer.IsTimeChanged && AudioDebugging.Enabled) {
            GD.Print("\t->CurrTime = " + currTime + " s. UpdateTime = " + updateTime + " s");
            GD.Print("\t->Current Function Being Played = " + currFunction);
            GD.Print("\t->Elapsed Time: " + (int) timer.ElapsedTime + " s " + "[absolute: "+ timer.ElapsedTime + " s]");
            GD.Print("\t->UpdateTime = " + updateTime + " s");
            GD.Print("\t->Playing Function " + (currFunction - 1) + " @ Function.Timer = " + currTime + " s");
            GD.Print("\t->Playing Function: " + functions[currFunction - 1].Name);
        }
    }

    /// <summary>
    /// Enables Timeline audio playback.
    /// </summary>
    /// <returns><c>true</c> if Timeline audio playback was succesfully enabled.</returns>
    public bool StartPlaying() {
        // Empty timeline case
        if(functions.Count == 0) return false;
        IsPlaying = true;
        SetProcess(true);
        timer.BeginTracking();
        Play();
        return true;
    }

    /// <summary>
    /// Disables Timeline audio playback.
    /// </summary>
    /// <returns><c>true</c> if Timeline audio playback was succesfully disabled.</returns>
    public bool StopPlaying() {
        // Poll the time and check if time has arrived. Stop sonificaiton if needed
		int currTime = timer.ClockTimeRounded;
        if(currTime != RunTime - 1) return false;
		
        // Stopping Sonification
        currFunction = 0;
        IsPlaying = false;
        SetProcess(false); 
        timer.Reset();
        return true;
    }

    /// <summary>
    /// Provides audio playback via <c> LowerTimeline.player </c>.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Sonfiy() {
        throw new NotImplementedException();        
    }

    /// <summary>
    /// Saves the current state of Timeline Audio playback (as an .mp3 file probably).
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Save() {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Resets the Timeline.
    /// </summary>
    /// <returns><c>true</c> if removal was successful.</returns>
    public bool Reset() {
        // Prevent deletion of an Active timeline
        if(IsPlaying) return false;

        // Defualts all relevant paramters
        timer.Reset();
        currFunction = 0;
        CurrPosition = 0;
        RunTime = 0;
        functions = new List<Function>();
        return true;
    }

    public override void _Process(double delta) {
        timer.Tick(delta);
        CurrPosition = timer.ClockTimeAbsolute;
		if(timer.IsTimeChanged && AudioDebugging.Enabled) GD.Print("\t->LowerTimeline Time t = " + timer.ClockTimeRounded + " s");
        Play();
    }
    
}