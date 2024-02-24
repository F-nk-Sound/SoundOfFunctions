using Godot;
using System;

public partial class FunctionPalette : Node
{
	private VBoxContainer _container;
	private Resource _textUpdateScript;
	public override void _Ready()
	{
		base._Ready();

		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<VBoxContainer>("FunctionContainer");

		Resource _textUpdateScript = GD.Load("res://UI/TextUpdate.cs");
	}

	private void OnButtonPressed()
	{
		Container sizeContainer = new Container();
		sizeContainer.CustomMinimumSize = new Vector2(225, 70);
        var FunctionContainer = GD.Load<PackedScene>("res://UI/FunctionContainer.tscn");
		var instance = FunctionContainer.Instantiate();
		sizeContainer.AddChild(instance);
		_container.AddChild(sizeContainer);
	}
}
