using Functions;
using Godot;
using System;

public partial class FunctionPalette : Node
{
	[Signal]
	public delegate void SelectedFunctionChangedEventHandler(FunctionPalette functionPalette);

	public IFunctionAST CurrentSelectedFunction { get; set; }

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
		Control instance = (Control) _functionContainer.Instantiate();
		instance.CustomMinimumSize = new Vector2(225, 70);
		_container.AddChild(instance);
	}
}
