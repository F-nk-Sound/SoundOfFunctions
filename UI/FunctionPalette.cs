using Godot;
using System;

public partial class FunctionPalette : Node
{
	private VBoxContainer _container;
	public override void _Ready()
	{
		base._Ready();

		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<VBoxContainer>("FunctionContainer");
	}

	private void OnButtonPressed()
	{
		TextEdit newFunction = new TextEdit();
		newFunction.CustomMinimumSize = new Vector2(225, 40);
		if (_container != null)
		{
            _container.AddChild(newFunction);
        }
	}
}
