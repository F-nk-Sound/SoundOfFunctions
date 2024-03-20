using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using System.Diagnostics;

using Functions;
using Functions.Sonification;

/// <summary>
/// Main Audio Generation Node. 
/// </summary>
public partial class AudioGenerator : Node {

	/// <summary>
	/// LowerTimeline representation of the Timeline UI Node
	/// </summary>
	public LowerTimeline timeline;
	
	/// <summary>
	/// Timer to manage and synchronize audio playback within the AudioGenerator. 
	/// </summary>
	private TimeKeeper timer;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {

		Initialize();
		AddTests();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		timer.Tick(delta);
		int currTime = timer.ClockTimeRounded;

		if(timer.IsTimeChanged) GD.Print("AudioGenerator Time t = " + currTime + " s");

		// Actually play functions
		if(currTime == timeline.GetFunction(0).StartTime && !timeline.IsPlaying) timeline.StartPlaying();
		if(currTime > timeline.GetFunction(0).StartTime && !timeline.IsPlaying) {
			GD.Print("That's all folks!");
			PrintTreePretty();
			GetTree().Quit();
		}
	
	}

	/// <summary>
	/// Initializes the Audio Generator.
	/// </summary>
	public void Initialize() {
		timer = new TimeKeeper();
		timeline = new LowerTimeline(this);
		timeline.SetProcess(false);
	}

	/// <summary>
	/// Add tests to the generator, duh.
	/// </summary>
	public void AddTests() {
		List<Function> funcList = new List<Function> {
			new Function("Hot Cross Buns", 5, 23, timeline),
			new Function("Scale", 23, 35, timeline),
			new Function("Seven Nation Army", 35, 56, timeline),
			new Function("Twinkle Twinkle", 56, 97, timeline),
			new Function("Lucid Dreams", 97, 137, timeline)
		};
		timeline.Add(funcList);
	}

}
