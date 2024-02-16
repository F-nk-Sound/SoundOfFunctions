using Godot;
using System;

// Notes (in Hz): Range A2 - B8
public static class NoteFrequency {
	public const float C4 = 261.63f; 
	public const float D4 = 293.66f; 
	public const float E4 = 329.66f; 
	public const float F4 = 349.23f; 
	public const float G4 = 392.00f; 
	public const float A4 = 440.00f; 
	public const float B4 = 493.88f; 
	public const float C5 = 523.35f; 
}

public partial class PlayFunctions : Node {
	public AudioStreamPlayer func1, func2, func3;
	private AudioStreamGeneratorPlayback playback1, playback2, playback3;
	private float sampleRate1, sampleRate2, sampleRate3;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		func1 = GetNode<AudioStreamPlayer>($"../Play1");

	
		AudioStreamGenerator gen1 = (AudioStreamGenerator) func1.Stream;


		sampleRate1 = gen1.MixRate;


		func1.Play();

		func3.Play();
		playback1 = (AudioStreamGeneratorPlayback) func1.GetStreamPlayback();
		playback2 = (AudioStreamGeneratorPlayback) func2.GetStreamPlayback();
		playback3 = (AudioStreamGeneratorPlayback) func3.GetStreamPlayback();
		FillBuffer1();
		FillBuffer2();
		FillBuffer3();
	}

	void FillBuffer1() {
		int frames = playback1.GetFramesAvailable();
		double phase = 0;
		float increment = NoteFrequency.C4 / sampleRate1;

		for (int i = 0; i < frames; i++) {
			playback1.PushFrame(Vector2.One * (float)(4 * Mathf.Floor(phase) - 2 * Mathf.Floor(2 * phase) + 1));
			phase += increment;
		}
	}

	void FillBuffer2() {
		int frames = playback2.GetFramesAvailable();
		double phase = 0;
		float increment = NoteFrequency.E4 / sampleRate2;

		for (int i = 0; i < frames; i++) {
			playback2.PushFrame(Vector2.One * (float)(4 * Mathf.Floor(phase) - 2 * Mathf.Floor(2 * phase) + 1));
			phase += increment;
		}
	}
	
		void FillBuffer3() {
		int frames = playback3.GetFramesAvailable();
		double phase = 0;
		float increment = NoteFrequency.G4 / sampleRate2;

		for (int i = 0; i < frames; i++) {
			playback3.PushFrame(Vector2.One * (float)(4 * Mathf.Floor(phase) - 2 * Mathf.Floor(2 * phase) + 1));
			phase += increment;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
	}
}
