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

		if(AudioDebugging.Enabled) 
		{
			AudioDebugging.Output("Displaying AudioGenerator SceneTree after adding intial LowerTimeline");
			PrintTreePretty();
		}
	}

	private void OnTimelineUpdated() {
		// Introduce the new timeline.
		AudioDebugging.Output("\tNew Timeline Processing Before Added as Child: " + timeline!.IsProcessing());
		foreach (Node n in timeline.GetChildren()) 
		{
			if (n is Function) n.SetProcess(false);
		}
		AudioDebugging.Output("\tNew Timeline Processing After Added as Child: " + timeline.IsProcessing());
		AudioDebugging.Output("Added the New LowerTimeline");

		if (AudioDebugging.Enabled) 
		{
			AudioDebugging.Output("Displaying Current AudioGenerator SceneTree after updating LowerTimeline.");
			PrintTreePretty();
			AudioDebugging.Output("Examining the processing of each child node");
			foreach (Node n in GetChildren()) 
			{
				AudioDebugging.Output("\tNode: " + n.Name + " Processing? " + n.IsProcessing());
				var grandkids = n.GetChildren();
				if (grandkids.Count != 0) 
				{
					AudioDebugging.Output("\tExamining the processing of each function node");
					foreach (Node gk in grandkids) 
					{
						if (gk is Function) AudioDebugging.Output("\t\tFunction: " + gk.Name + " Processing? " + gk.IsProcessing());
					}
				}
			}
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
		AudioDebugging.Output("AudioGenerator.Play(): IsPlaying = " + IsPlaying);
		if(!IsPlaying) {
			timeline!.StartPlaying();
		}
	}

}
