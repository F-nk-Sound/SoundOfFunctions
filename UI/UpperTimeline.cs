using Godot;
using Godot.Collections;
using Sonification;
using UI.Palette;

namespace UI;

public partial class UpperTimeline : Control
{

	// Play button for start/stop.
	[Export]
	Button? playButton;
	
	private string playIcon = "res://Assets/FOTS UI/icons8-circled-play-50.png";
	private string stopIcon = "res://Assets/FOTS UI/esper-icon-stop.png";
	
	[Export]
	public HBoxContainer? timelineContainer;

	// I believe I'm gonna need to reference the functionPalette's drag and drop
	[Export]
	private FunctionPalette? functionPalette;

	[Export]
	private AudioGenerator? audioGenerator;
	// Called when the node enters the scene tree for the first time.

	[Export]
	LowerTimeline? lowerTimeline;

	[Export]
	PackedScene? timelineFunctionContainer;

	/// <summary>
	/// Godot event called when seeking is requested.
	/// </summary>
	[Signal]
	public delegate void SeekingRequestedEventHandler(string type);

	private void OnButtonPressed()
	{
		Container sizeContainer = new()
		{
			CustomMinimumSize = new Vector2(225, 70)
		};
		var FunctionContainer = GD.Load<PackedScene>("res://UI/TimelineContainer.tscn");
		var instance = FunctionContainer.Instantiate();
		sizeContainer.AddChild(instance);
		timelineContainer?.AddChild(sizeContainer);
	}

	/// <summary>
	/// Handles the event where it has been indicated that audio playback should begin.
	/// </summary>
	private void OnToolbarPlayButtonPressed()
	{

		// Grab the Audio Generator from the scene tree and begin/end playback.
		if(!audioGenerator!.timeline!.IsPlaying && audioGenerator!.timeline!.Count > 0) 
		{
			audioGenerator.Play();
			playButton!.Icon = (Texture2D) GD.Load(stopIcon);
		}
		else 
		{
			audioGenerator.Stop();
			playButton!.Icon = (Texture2D) GD.Load(playIcon);
		}
	}

	private void OnToolBarPreviousButtonPressed() 
	{
		if(audioGenerator!.timeline!.IsPlaying)
		{	
			audioGenerator.timeline.SeekBackward = true;
			if(audioGenerator.timeline.SeekBackward) EmitSignal(SignalName.SeekingRequested, "<<");
		}
	}

	private void OnToolBarNextButtonPressed() 
	{
		if(audioGenerator!.timeline!.IsPlaying)
		{	
			audioGenerator.timeline.SeekForward = true;
			if(audioGenerator.timeline.SeekForward) EmitSignal(SignalName.SeekingRequested, ">>");
		}
	}

	private void OnLowerTimelineAudioPlaybackFinished()
	{
		playButton!.Icon = (Texture2D) GD.Load(playIcon);
	}

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		if (data.As<Function>() is Function f)
		{
			return true;
		}
		return false;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		if (data.As<Function>() is Function f)
		{
			lowerTimeline!.Add(f);
			TimelineFunctionContainer container = timelineFunctionContainer!.Instantiate<TimelineFunctionContainer>();
			container.StartTime = f.StartTime;
			container.EndTime = f.EndTime;
			container.LatexString = f.FunctionAST!.Latex;
			container.Timeline = lowerTimeline;
			container.Index = lowerTimeline.Count - 1;
			timelineContainer!.AddChild(container);
		}
	}

}
