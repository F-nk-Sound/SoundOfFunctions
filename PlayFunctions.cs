using Godot;
using System;

public partial class PlayFunctions : Node
{
	[Export] public AudioStreamPlayer Player;

	private AudioStreamGeneratorPlayback playback;
	private float sampleRate;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AudioStreamGenerator gen = (AudioStreamGenerator)Player.Stream;
		sampleRate = gen.MixRate;
		Player.Play();
		playback = (AudioStreamGeneratorPlayback)Player.GetStreamPlayback();
		FillBuffer();
	}

	void FillBuffer()
	{
		int frames = playback.GetFramesAvailable();
		double phase = 0;
		float increment = 440 / sampleRate;

		for (int i = 0; i < frames; i++)
		{
			playback.PushFrame(Vector2.One * (float)(4 * Mathf.Floor(phase) - 2 * Mathf.Floor(2 * phase) + 1));
			phase += increment;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
