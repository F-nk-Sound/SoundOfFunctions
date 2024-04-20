using Godot;
using Functions;
using Parsing;

namespace Sonification;

/// <summary>
/// Main Audio Generation Node. 
/// </summary>
public partial class AudioGenerator : Node {

	/// <summary>
	/// LowerTimeline representation of the Timeline UI Node
	/// </summary>
	[Export]
	public LowerTimeline? timeline;

	/// <summary>
	/// If <c> true </c>, AudioGenerator playback is active.
	/// </summary>
	public bool IsPlaying => timeline!.IsPlaying;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Add and prep the Timeline.
		timeline!.SetProcess(false);

	}

	private void OnTimelineUpdated() {
		// Introduce the new timeline.
		foreach (Node n in timeline!.GetChildren()) 
		{
			if (n is Function) n.SetProcess(false);
		}

	}

	/// <summary>
	/// Handles the case where the timeline has indicated audio playback is finished.
	/// </summary>
	private void OnAudioPlaybackFinished() {
		GD.Print("---Playback Over---");
	}

	/// <summary>
	/// Begins AudioGenerator audio playback from the beginning of the Timeline.
	/// </summary>
	public void Play() {
		AudioDebugging.Output("AudioGenerator.Play() Activated");
		if(!IsPlaying) {
			timeline!.StartPlaying();
		}
	}

	public void Stop() {
		AudioDebugging.Output("AudioGenerator.Stop() Activated");
		if(IsPlaying) {
			timeline!.StopPlaying(true);
		}
	}

}
