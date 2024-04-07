using Godot;
using System;

using Sonification;

namespace UI;

public partial class UpperTimeline : Node2D
{
	private HBoxContainer? _container;
	private FunctionPalette? functionPalette; // I believe I'm gonna need to reference the functionPalette's drag and drop
	// Called when the node enters the scene tree for the first time.
	
	private AudioGenerator? audioGenerator;
	public override void _Ready()
	{
		base._Ready(); // I saw Mark had this
		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<HBoxContainer>("TimelineContainer");
		audioGenerator = GetNode<AudioGenerator>("AudioGenerator");

		// Connect the necessary signals from the ToolBar Node
		GetParent().GetNode<Toolbar>("Toolbar").PlayButtonPressed += OnToolbarPlayButtonPressed;
	}

	private void OnButtonPressed()
	{
		Container sizeContainer = new Container();
		sizeContainer.CustomMinimumSize = new Vector2(225, 70);
		var FunctionContainer = GD.Load<PackedScene>("res://UI/TimelineContainer.tscn");
		var instance = FunctionContainer.Instantiate();
		sizeContainer.AddChild(instance);
		_container.AddChild(sizeContainer);
	}

	/// <summary>
	/// Handles the event where it has been indicated that audio playback should begin.
	/// </summary>
	private void OnToolbarPlayButtonPressed() 
	{
		// Grab the Audio Generator from the scene tree and begin playback.
		AudioGenerator generator = GetParent().GetNode<AudioGenerator>($"Timeline/AudioGenerator");
		if(generator != null) {
			generator.Play();
			AudioDebugging.Output("AudioGenerator.Play() Activated");
		}
	}

}


