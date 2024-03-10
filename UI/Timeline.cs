using Godot;
using System;

public partial class Timeline : Node2D
{
	private HBoxContainer _container;
	private FunctionPalette functionPalette; // I believe I'm gonna need to reference the functionPalette's drag and drop
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // I saw Mark had this
		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<HBoxContainer>("TimelineContainer");
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

}


