using Functions;
using Godot;
using Godot.Collections;
using Graphing;
using Sonification;
using System;

namespace UI.Palette;

/// <summary>
/// UI Node that hold the function palette.
/// </summary>
public partial class FunctionPalette : Node
{
	[Export]
	FunctionRenderer? renderer;

	/// <summary>
	/// Godot Event called when the current function selected by the user
	/// has changed.
	/// </summary>
	[Signal]
	public delegate void SelectedFunctionChangedEventHandler();

	/// <summary>
	/// Godot Event called when the current function has been dragged.
	/// </summary>
	/// <param name="position">The current global position of the function.</param>
	[Signal]
	public delegate void FunctionDraggedEventHandler(Vector2 position);

	/// <summary>
	/// Godot Event called when the current function is being dragged.
	/// </summary>
	/// <param name="position">The current global position of the funtion.</param>
	[Signal]
	public delegate void FunctionDraggingEventHandler(Vector2 position);

	/// <summary>
	/// C# Event called when the current function selected by the user
	/// has changed.
	/// </summary>
	/// <param name="function">Current selected function, as an IFunctionAST.</param>
	public delegate void C_SelectedFunctionEventHandler(Function function);
	public event C_SelectedFunctionEventHandler? C_SelectedFunctionChanged;

	/// <summary>
	/// C# Event called when the current function has been dragged.
	/// </summary>
	/// <param name="position">The current global position of the function.</param>
	/// <param name="function">Current selected function, as an IFunctionAST.</param>
	public delegate void C_FunctionDraggedEventHandler(Vector2 position, Function function);
	public event C_FunctionDraggedEventHandler? C_FunctionDragged;

	/// <summary>
	/// C# Event called when the current function is being dragged.
	/// </summary>
	/// <param name="position">The current global position of the function.</param>
	/// <param name="function">Current selected function, as a Function from Sonification.</param>
	public delegate void C_FunctionDraggingEventHandler(Vector2 position, Function function);
	public event C_FunctionDraggingEventHandler? C_FunctionDragging;

	private Function? currentSelectedFunction;
	public Function CurrentSelectedFunction
	{
		get { return currentSelectedFunction!; }
		set
		{
			if (value == null) return;
			currentSelectedFunction = value;
			EmitSignal(SignalName.SelectedFunctionChanged);
			C_SelectedFunctionChanged?.Invoke(value);
		}
	}

	[Export]
	private VBoxContainer? _container;
	[Export]
	private Resource? _textUpdateScript;
	[Export]
	private PackedScene? _functionContainer;
	public override void _Ready()
	{
		base._Ready();
	}

	/// <summary>
	/// Called when the '+' is pressed in the UI.
	/// </summary>
	private void OnButtonPressed()
	{
		AddFunction();
	}

	private FunctionContainer AddFunction()
	{
		if (_container == null || _functionContainer == null) return null;
		FunctionContainer instance = (FunctionContainer)_functionContainer.Instantiate();
		instance.FunctionPalette = this;
		_container.AddChild(instance);
		return instance;
	}

	private FunctionContainer AddFunction(string textRepresentation, float startTime, float endTime)
	{
		FunctionContainer functionContainer = AddFunction();
		functionContainer.FunctionUpdate(textRepresentation, startTime, endTime);
		return functionContainer;
	}

	/// <summary>
	/// Calls C# and Godot dragged events.
	/// </summary>
	/// <param name="position">The global position of the function.</param>
	public void OnDraggedEvent(Vector2 position)
	{
		if (CurrentSelectedFunction == null) return;
		C_FunctionDragged?.Invoke(position, CurrentSelectedFunction);
		EmitSignal(SignalName.FunctionDragged, position);
	}

	/// <summary>
	/// Calls C# and Godot dragging events.
	/// </summary>
	/// <param name="position">The global position of the function.</param>
	public void OnDraggingEvent(Vector2 position)
	{
		if (CurrentSelectedFunction == null) return;
		C_FunctionDragging?.Invoke(position, CurrentSelectedFunction);
		EmitSignal(SignalName.FunctionDragging, position);
	}

	/// <summary>
	/// Saves all data elements needed to create a FunctionPalette Node to a Godot Dictionary.
	/// </summary>
	/// <returns>Returns the Godot Dictionary that holds the required information.</returns>
	public Dictionary Save() {
		var res = new Dictionary();
		Array<Dictionary<string, Variant>> functionStings = new Array<Dictionary<string, Variant>>();
		Array<Node> functionNodes = _container!.GetChildren();
		foreach (var functionNode in functionNodes)
		{
			FunctionContainer fc = (FunctionContainer) functionNode;
			if (fc == null) continue;
			Dictionary<string, Variant> functionDictionary = new Dictionary<string, Variant>();
			functionDictionary.Add("TextRepresentation", fc.Function.TextRepresentation!);
			functionDictionary.Add("StartTime", fc.Function.StartTime);
			functionDictionary.Add("EndTime", fc.Function.EndTime);
			functionStings.Add(functionDictionary);
		}
		res.Add("Functions", functionStings);
		return res;
	}

	public void Load(Dictionary dictionary)
	{
		//GD.Print(dictionary);
		foreach (var child in  _container!.GetChildren())
		{
			child.QueueFree();
		}
		Array<Dictionary<string, Variant>> functions;
		if (!dictionary.TryGetValue("Functions", out Variant functionsListVariant)) return;
		functions = (Array<Dictionary<string, Variant>>) functionsListVariant;
		foreach (var function in functions)
		{
			AddFunction(
					(string) function["TextRepresentation"],
					(float) function["StartTime"],
					(float) function["EndTime"]
				);
		}
	}

	public void GraphFunction(Function fn)
	{
		if (fn == null) return;
		renderer!.SetFunction(fn);
	}
}
