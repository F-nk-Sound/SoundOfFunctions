using System;
using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Sonification;

/// <summary>
/// Backend of the Timeline UI. This is the backbone of Audio Generation. 
/// </summary>
public partial class LowerTimeline : Node
{
	/// <summary>
	/// Plays the Audio of the Timeline using <c> LowerTimeline.Sonify() </c>.
	/// </summary>
	private AudioStreamPlayer player;

	/// <summary>
	/// List of functions on the Timeline.
	/// </summary>
	[JsonRequired]
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
	public double RunTime => functions.Select(f => f.RunTime).Sum();

	/// <summary>
	/// Number of Functions currently held within the LowerTimeline.
	/// </summary>
	public int Count
	{
		get { return functions.Count; }
	}

	/// <summary>
	/// If <c> true </c>, Timeline audio playback is active.
	/// </summary>
	public bool IsPlaying { get; set; }

	/// <summary>
	/// Current position of Timeline audio playback (in seconds).
	/// </summary>
	public double CurrPosition { get; set; }

	/// <summary>
	/// Godot event called when audio playback has finished.
	/// </summary>
	[Signal]
	public delegate void AudioPlaybackFinishedEventHandler();

	/// <summary>
	/// Initializes a new LowerTimeline Node.
	/// </summary>
	/// <param name="parent">The parent of this LowerTimeline node.</param>
	public LowerTimeline()
	{
		functions = new();
		timer = new();

		Reset();

		// Scene Tree stuff
		player = new AudioStreamPlayer
		{
			Stream = new AudioStreamGenerator(),
			Name = "LowerTimelinePlayer"
		};

		AddChild(player);
	}

	/// <summary>
	/// Fully reset the state of the timeline
	/// </summary>
	public void Reset()
	{
		currFunction = -1; // Initialized to -1 to indicate playing hasn't begun yet. 0 indexed quantity.
		CurrPosition = 0.0;
		IsPlaying = false;
		functions.Clear();
		timer.Reset();
		SetProcess(false);
	}

	/// <summary>
	/// Adds a Function Node to the end of the Timeline.
	/// </summary>
	/// <param name="func">Function to be added.</param>
	public void Add(Function func)
	{
		AudioDebugging.Output("Function Processing Pre Added To TL: " + func.IsProcessing());
		if (!IsAncestorOf(func))
			AddChild(func);
		func.SetProcess(false);
		functions.Add(func);
		AudioDebugging.Output("Function Processing Pre Added To TL: " + func.IsProcessing());
		AudioDebugging.Output("Added to LowerTimeline: " + func.Name);
	}

	/// <summary>
	/// Adds a list of Functions to the end of the Timeline.
	/// </summary>
	/// <param name="funcList">List of Functions to be added.</param>
	public void Add(List<Function> funcList)
	{
		foreach (Function f in funcList) Add(f);
	}

	/// <summary>
	/// Grabs the specified Function from the Timeline.
	/// </summary>
	/// <param name="index">Location of Function on the Timeline (0 indexed).</param>
	/// <returns>Function a the specified location.</returns>
	public Function GetFunction(int index)
	{
		return functions[index];
	}

	/// <summary>
	/// Removes the specified Function from the Timeline.
	/// </summary>
	/// <param name="index">Location of Function on the Timeline (0 indexed).</param>
	/// <returns><c>true</c> if removal was successful.</returns>
	public bool RemoveFunction(int index)
	{
		Function func = GetFunction(index);
		if (func == null) return false;
		functions.Remove(func);
		return true;
	}

	/// <summary>
	/// Plays the current function of the Timeline at the appropriate time.
	/// </summary>
	private void Play()
	{
		// Stop playback if necessary
		AudioDebugging.Output("\tEntered LowerTimeline.Play()");
		if (StopPlaying() || currFunction >= functions.Count) return;

		// Grab the current timer position and the time to allow the next function to play
		int currTime = timer.ClockTimeRounded;
		double updateTime = (currFunction == -1) ? functions.First().RunTime : functions[currFunction].RunTime;

		// Play the functions within the timeline at the appropriate time
		if (currFunction == -1 || timer.ElapsedTime >= updateTime)
		{
			currFunction++;
			functions[currFunction].StartPlaying();
			timer.ResetTracking();
		}

		AudioDebugging.Output("\t->Timeline.Timer.CurrTime = " + currTime + " s. UpdateTime/StopTime = " + updateTime + " s");
		AudioDebugging.Output("\t->Timeline.Timer.ElapsedTime: " + (int)timer.ElapsedTime + " s " + "[absolute: " + timer.ElapsedTime + " s]");
		AudioDebugging.Output("\t->Playing Timeline.CurrFunction " + currFunction + ":" + functions[currFunction].Name + " @ Function.Timer = " + currTime + " s");
		AudioDebugging.Output("\tExit LowerTimeline.Play()");
	}

	/// <summary>
	/// Enables Timeline audio playback.
	/// </summary>
	/// <returns><c>true</c> if Timeline audio playback was succesfully enabled.</returns>
	public bool StartPlaying()
	{
		AudioDebugging.Output("Entered LowerTimeline.StartPlaying():");
		bool res = true;
		if (functions.Count == 0 || IsPlaying) res = false;
		else
		{
			IsPlaying = true;
			SetProcess(true);
			timer.BeginTracking();
			Play();
		}

		AudioDebugging.Output("Exit LowerTimeline.StartPlaying(): returns " + res);
		return res;
	}

	/// <summary>
	/// Disables Timeline audio playback.
	/// </summary>
	/// <returns><c>true</c> if Timeline audio playback was succesfully disabled.</returns>
	public bool StopPlaying()
	{
		bool res = true;
		AudioDebugging.Output("\t\tEntered LowerTimeline.StopPlaying():");
		AudioDebugging.Output("\t\tLowerTimeline.Timer.ClockTimeRounded == " + timer.ClockTimeRounded);
		AudioDebugging.Output("\t\tLowerTimeline.RunTime == " + RunTime);
		// Poll the time and check if time has arrived. Stop Sonifcation if necessary.
		if (timer.ClockTimeRounded < RunTime) res = false;
		else
		{
			// Stop Sonification.
			currFunction = -1;
			IsPlaying = false;
			SetProcess(false);
			timer.Reset();
			EmitSignal(SignalName.AudioPlaybackFinished);
		}

		AudioDebugging.Output("\t\tExit LowerTimeline.StopPlaying(): returns " + res);
		return res;
	}

	/// <summary>
	/// Provides audio playback via <c> LowerTimeline.player </c>.
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	public void Sonify()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Saves all data elements needed to create a LowerTimeline to a Godot Dictionary.
	/// </summary>
	/// <returns>Returns the Godot Dictionary that holds the required information.</returns>
	public Dictionary Save()
	{
		// Retrieve and store all functions within the timeline as JSON.
		Dictionary functionsDictionary = new();
		foreach (var (func, i) in functions.Select((f, i) => (f, i)))
		{
			var functionData = func.Save();
			functionsDictionary.Add(i, functionData);
		}

		var res = new Dictionary {
			{ "Functions", functionsDictionary },
			{ "Count", Count },
			{ "Name", Name }
		};

		return res;
	}

	public void Display()
	{
		GD.Print("LowerTimeline: " + Name);
		foreach (Function func in functions)
		{
			func.Info();
		}
		GD.Print();
	}

	public override void _Process(double delta)
	{
		timer.Tick(delta);
		CurrPosition = timer.ClockTimeAbsolute;
		Play();
	}
}
