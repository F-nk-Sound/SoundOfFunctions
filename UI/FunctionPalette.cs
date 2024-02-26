using Functions;
using Godot;
using System;

public partial class FunctionPalette : Node
{
	public IFunctionAST CurrentSelectedFunction { get; private set; }

	private VBoxContainer? _container;
	private Resource? _textUpdateScript;
	private PackedScene? _functionContainer;
	public override void _Ready()
	{
		base._Ready();

		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<VBoxContainer>("FunctionsContainer");

		Resource _textUpdateScript = GD.Load("res://UI/TextUpdate.cs");
		_functionContainer = GD.Load<PackedScene>("res://UI/FunctionContainer.tscn");

	}

	private void OnButtonPressed()
	{
		if (_container == null || _functionContainer == null) return;
		Control sizeContainer = new Control();
		sizeContainer.CustomMinimumSize = new Vector2(225, 70);
		Control instance = (Control) _functionContainer.Instantiate();
		instance.CustomMinimumSize = new Vector2(225, 70);
		//sizeContainer.AddChild(instance);
		_container.AddChild(instance);
	}
}
