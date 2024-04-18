using Godot;
using Godot.Collections;
using Sonification;
using UI.Palette;

namespace UI;

public partial class UpperTimeline : Control
{
	[Export]
	private HBoxContainer? timelineContainer;

	// I believe I'm gonna need to reference the functionPalette's drag and drop
	[Export]
	private FunctionPalette? functionPalette; 

	[Export]
	private AudioGenerator? audioGenerator;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		base._Ready(); // I saw Mark had this
	}

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
		// Grab the Audio Generator from the scene tree and begin playback.
		audioGenerator!.Play();
		AudioDebugging.Output("AudioGenerator.Play() Activated");
	}

	/// <summary>
	/// Saves all data elements needed to create an UpperTimeline Node to a Godot Dictionary.
	/// </summary>
	/// <returns>Returns the Godot Dictionary that holds the required information.</returns>
	public Dictionary Save() 
	{
		var res = new Dictionary();
		return res;
	}
}


