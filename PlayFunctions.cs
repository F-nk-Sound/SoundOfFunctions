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
public partial class PlayFunctions : Node {

	private List<AudioStreamPlayer> strmPlayerList = new List<AudioStreamPlayer>();
	private List<AudioStreamGenerator> strmGenList = new List<AudioStreamGenerator>();

	static void PlayNoteSimple(AudioStreamPlayer player, int n) {

		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		player.Play();
		AudioStreamGeneratorPlayback playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
		
		double phase = 0.0;
		int frames = playback.GetFramesAvailable();
		double increment = Frequencies.Notes[n] / sampleRate;
		for (int i = 0; i < frames; i++) {
			playback.PushFrame(Vector2.One * (float)(4 * Mathf.Sin(phase * Mathf.Tau)));
			phase += increment;
		}	
	}

	public Vector2[] generateTone(AudioStreamPlayer player, double dur, int noteNum) {
		
		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		((AudioStreamGenerator) player.Stream).BufferLength = (float) (dur * dur/8);
		int bufferSize = (int) (sampleRate * dur * dur/8);
		Vector2[] audio = new Vector2[bufferSize];
		double increment = Frequencies.Notes[noteNum] / sampleRate;
		var phase = 0.0;
		for (int i = 0; i < bufferSize; i++) {
			var sample = Vector2.One * (float) (4 * Mathf.Sin(Mathf.Tau * phase));
			audio[i] = sample;
			phase += increment;
		}	
	
		return audio;
	}

	public void playTone(AudioStreamPlayer player, Vector2[] audio) {
		player.Play();
		var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
		playback.PushBuffer(audio);
	}

	Timeline sequence = new Timeline();
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {
		
		Function linear = new Function("linear", "y = 2x + 6", 3, 5);
		Function cubic = new Function("cubic", "y = x^3 + 2", 5, 10);
		
		// Add the functions to the scene tree
		linear.Name = "linear";
		cubic.Name = "cubic";
		GetNode($".").AddChild(linear);
		GetNode($".").AddChild(cubic);

		// Add Functions to timeline
		linear.SetProcess(false);
		cubic.SetProcess(false);
		sequence.AddFunction(linear);
		sequence.AddFunction(cubic);
	
	}

	TimeKeeper timer = new TimeKeeper();
	bool first = false;
	bool second = false;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		timer.Tick(delta);
		int currTime = timer.GetRoundedClockTime();
		bool changed = timer.GetTimeChanged();
		if(!changed) return;

		// Actually play functions
		if(currTime == 5 && !first) sequence.GetFunction(0).SetProcess(true);
		if(currTime == 10) sequence.GetFunction(0).SetProcess(false);
		if(currTime == 15 && !first) sequence.GetFunction(1).SetProcess(true);
		if(currTime == 20) sequence.GetFunction(1).SetProcess(false);
		GD.Print("This is from the PlayFunctions Class @ t = " + currTime);

	}

	public void ActivateProcess() {
		
	}
}
