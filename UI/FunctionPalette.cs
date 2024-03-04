using Functions;
using Godot;
using System;

public partial class FunctionPalette : Node
{
	[Signal]
	public delegate void SelectedFunctionChangedEventHandler();
    [Signal]
    public delegate void FunctionDraggedEventHandler(Vector2 position);
	[Signal]
    public delegate void FunctionDraggingEventHandler(Vector2 position);

	public delegate void C_SelectedFunctionEventHandler(IFunctionAST functionAST);
	public event C_SelectedFunctionEventHandler? C_SelectedFunctionChanged;
    public delegate void C_FunctionDraggedEventHandler(Vector2 position, IFunctionAST functionAST);
    public event C_FunctionDraggedEventHandler? C_FunctionDragged;
    public delegate void C_FunctionDraggingEventHandler(Vector2 position, IFunctionAST functionAST);
    public event C_FunctionDraggingEventHandler? C_FunctionDragging;

	private IFunctionAST? currentSelectedFunction;
	public IFunctionAST CurrentSelectedFunction
	{
		get { return currentSelectedFunction; }
		set
		{
			if (value == null) return;
			currentSelectedFunction = value;
			EmitSignal(SignalName.SelectedFunctionChanged);
			C_SelectedFunctionChanged?.Invoke(value);
		}
	}

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

	public void OnDraggedEvent(Vector2 position)
	{
		if (CurrentSelectedFunction == null) return;
		C_FunctionDragged?.Invoke(position, CurrentSelectedFunction);
        EmitSignal(SignalName.FunctionDragged, position);
    }

    public void OnDraggingEvent(Vector2 position)
    {
        if (CurrentSelectedFunction == null) return;
        C_FunctionDragging?.Invoke(position, CurrentSelectedFunction);
        EmitSignal(SignalName.FunctionDragging, position);
    }
}
