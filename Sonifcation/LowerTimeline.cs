using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

	ImmutableList<Function>? cachedFunctions;

	/// <summary>
	/// List of functions on the Timeline.
	/// </summary>
	private ImmutableList<Function>? Functions => cachedFunctions;

	[Export]
	HBoxContainer? timelineContainer;

	[Export]
	PackedScene? timelineFunctionContainer;

	[Export]
	ProgressBar? progressBar;

	/// <summary>
	/// Timer to manage and synchronize audio playback within the Timeline.
	/// </summary>
	private TimeKeeper timer;

	/// <summary>
	/// Current function being played by the Timeline.
	/// </summary>

	int _currFunction;
	private int CurrFunction 
	{
		get => _currFunction;
		set
		{
			_currFunction = value;
		}
	}

	/// <summary>
	/// Runtime of the Timeline (in seconds).
	/// </summary>    
	public double RunTime => Functions!.Select(f => f.RunTime).Sum();

	/// <summary>
	/// Number of Functions currently held within the LowerTimeline.
	/// </summary>
	public int Count
	{
		get { return Functions!.Count; }
	}

	/// <summary>
	/// If <c> true </c>, Timeline audio playback is active.
	/// </summary>
	public bool IsPlaying { get; set; }

	private bool _next = false;
	public bool SeekForward
	{
		get { return _next; }
		set { if (CurrFunction + 1 < Functions!.Count && value == true) _next = value; }
	}

	private bool _previous = false;
	public bool SeekBackward
	{
		get { return _previous; }
		set
		{
			AudioDebugging.Output("-->Setting Seek Backward: CurrFunction = " + CurrFunction);
			AudioDebugging.Output("-->Condition: [currFunction > 0 && value == true] is " + ((CurrFunction > 0) && value == true));
			if (CurrFunction > 0 && value == true) _previous = value;
		}
	}

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
		timer = new();
		cachedFunctions = ImmutableList.Create<Function>();

		Reset();

		// Scene Tree stuff
		player = new AudioStreamPlayer
		{
			Stream = new AudioStreamGenerator(),
			Name = "LowerTimelinePlayer"
		};

		AddChild(player);
	}

	public void ComputeFunctions()
	{
		cachedFunctions =
			timelineContainer!
			.GetChildren()
			.Select(c => ((TimelineFunctionContainer)c).Function!)
			.ToImmutableList();

		progressBar!.MaxValue = 
			timelineContainer
			.GetChildren()
			.Cast<Control>()
			.Select(c => c.Size.X)
			.Sum();
	}

	/// <summary>
	/// Fully reset the state of the timeline
	/// </summary>
	public void Reset()
	{
		CurrFunction = -1; // Initialized to -1 to indicate playing hasn't begun yet. 0 indexed quantity.
		IsPlaying = false;
		if (timelineContainer is not null)
			foreach (var child in timelineContainer!.GetChildren())
			{
				timelineContainer.RemoveChild(child);
				child.QueueFree();
			}
		foreach (var child in GetChildren().OfType<TimelineFunctionContainer>())
		{
			RemoveChild(child);
			child.QueueFree();
		}
		timer.Reset();
	}

	/// <summary>
	/// Adds a Function Node to the end of the Timeline.
	/// </summary>
	/// <param name="func">Function to be added.</param>
	public void Insert(Function func, int index)
	{
		if (!IsAncestorOf(func))
			AddChild(func);

		TimelineFunctionContainer container = timelineFunctionContainer!.Instantiate<TimelineFunctionContainer>();
		container.Initialize(func, this);

		timelineContainer!.AddChild(container);
		timelineContainer.MoveChild(container, index);

		func.SetProcess(false);
	}

	public void Add(Function func)
	{
		Insert(func, Count);
	}

	/// <summary>
	/// Adds a list of Functions to the end of the Timeline.
	/// </summary>
	/// <param name="funcList">List of Functions to be added.</param>
	public void Add(List<Function> funcList)
	{
		foreach (Function f in funcList)
			Add(f);
	}

	/// <summary>
	/// Grabs the specified Function from the Timeline.
	/// </summary>
	/// <param name="index">Location of Function on the Timeline (0 indexed).</param>
	/// <returns>Function a the specified location.</returns>
	public Function GetFunction(int index)
	{
		return Functions![index];
	}

	/// <summary>
	/// Plays the current function of the Timeline at the appropriate time.
	/// </summary>
	private void Play()
	{
		// Stop playback if necessary
		AudioDebugging.Output("\tEntered LowerTimeline.Play(). ToPrev = " + SeekBackward + "/ToNext =" + SeekForward);
		if (StopPlaying(false) || CurrFunction == Functions!.Count) return;

		// Grab the current timer position and the time to allow the next function to play
		int currTime = timer.ClockTimeRounded;
		double updateTime = (CurrFunction == -1) ? Functions!.First().RunTime : Functions![CurrFunction].RunTime;

		// Play the functions within the timeline at the appropriate time
		if (CurrFunction == -1 || timer.ElapsedTime >= updateTime)
		{
			CurrFunction++;
			Functions![CurrFunction].StartPlaying();
			timer.ResetTracking();
		}

		AudioDebugging.Output("\t->Timeline.Timer.CurrTime = " + currTime + " s. UpdateTime/StopTime = " + updateTime + " s");
		AudioDebugging.Output("\t->Timeline.Timer.ElapsedTime: " + (int)timer.ElapsedTime + " s " + "[absolute: " + timer.ElapsedTime + " s]");
		AudioDebugging.Output("\t->Playing Timeline.CurrFunction " + CurrFunction + ":" + Functions![CurrFunction].TextRepresentation);
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
		if (Functions!.Count == 0 || IsPlaying)
			res = false;
		else
		{
			IsPlaying = true;
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
	public bool StopPlaying(bool interrupted)
	{
		bool res = true;
		AudioDebugging.Output("\tEntered LowerTimeline.StopPlaying():");
		AudioDebugging.Output("\t\tLowerTimeline.RunTime == " + RunTime);
		AudioDebugging.Output("\t\tLowerTimeline.Timer.time == " + timer.ClockTimeRounded);
		// Poll the time and check if time has arrived. Stop Sonifcation if necessary.
		if (timer.ClockTimeRounded < RunTime && !interrupted) res = false;
		else
		{
			// Stop Sonification.
			if (CurrFunction != Functions!.Count)
				Functions[CurrFunction].StopPlaying();
			CurrFunction = -1;
			IsPlaying = false;
			timer.Reset();
			EmitSignal(SignalName.AudioPlaybackFinished);
		}

		AudioDebugging.Output("\tExit LowerTimeline.StopPlaying(): returns " + res);
		return res;
	}

	/// <summary>
	/// Saves all data elements needed to create a LowerTimeline to a Godot Dictionary.
	/// </summary>
	/// <returns>Returns the Godot Dictionary that holds the required information.</returns>
	public Dictionary Save()
	{
		// Retrieve and store all functions within the timeline as JSON.
		Dictionary functionsDictionary = new();
		foreach (var (func, i) in Functions!.Select((f, i) => (f, i)))
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

	private void ToPreviousFunction()
	{
		AudioDebugging.Output("Entered ToPreviousFunction");
		AudioDebugging.Output("\tCurrFunction = " + CurrFunction + ":" + Functions![CurrFunction].TextRepresentation);
		AudioDebugging.Output("\t\tCurrFunction.Runtime = " + Functions[CurrFunction].RunTime);
		AudioDebugging.Output("\tPrevFunction = " + (CurrFunction - 1) + ":" + Functions[CurrFunction - 1].TextRepresentation);
		AudioDebugging.Output("\t\tPrevFunction.Runtime = " + Functions[CurrFunction - 1].RunTime);
		AudioDebugging.Output("\tLowerTimeline.Timer.CurrTime = " + timer.ClockTimeAbsolute);

		_previous = false;
		Functions[CurrFunction].StopPlaying();
		double startTime = 0;
		if (CurrFunction - 1 != 0)
			for (int i = 0; i < CurrFunction - 1; i++)
				startTime += Functions[i].RunTime;
		timer.Reset(startTime);
		timer.BeginTracking();
		CurrFunction--;
		Functions[CurrFunction].StartPlaying();

		AudioDebugging.Output("\tLowerTimeline.Timer.CurrTime (assigned new Starttime) = " + timer.ClockTimeAbsolute);
		AudioDebugging.Output("Exited ToPreviousFunction");
	}

	private void ToNextFunction()
	{
		_next = false;
		Functions![CurrFunction].StopPlaying();
		double startTime = 0;
		for (int i = 0; i <= CurrFunction; i++) 
			startTime += Functions[i].RunTime;
		timer.Reset(startTime);
		timer.BeginTracking();
		CurrFunction++;
		Functions[CurrFunction].StartPlaying();
	}

	private void OnSeekingRequested(string type)
	{
		if (type.Equals("<<")) ToPreviousFunction();
		else if (type.Equals(">>")) ToNextFunction();
	}

	public override void _Process(double delta)
	{
		if (IsPlaying)
		{
			timer.Tick(delta);
			Play();

			//GD.Print("Elapsed: ", timer.ElapsedTime);
			//GD.Print("Runtime: ", Functions![CurrFunction].RunTime);
		}

		if (progressBar is not null)
		{
			if (CurrFunction is not -1)
			{
				Control container = timelineContainer!.GetChild<Control>(CurrFunction);
				progressBar.Value = container.Position.X + 
					(container.Size.X * (timer.ElapsedTime / Functions![CurrFunction].RunTime));
			}
			else
			{
				progressBar.Value = 0;
			}
		}
	}
}
