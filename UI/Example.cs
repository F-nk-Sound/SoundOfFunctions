using Functions;
using Godot;
using System;
using UI.Palette;

using UI;
using Sonification;

public partial class Example : Node
{
	[Export]
	private FunctionPalette? functionPalette;

	public override void _Ready()
	{
		base._Ready();
	}

	/// <summary>
	/// Because of the line in _Ready, called whenever the selected
	/// function is changed.
	/// </summary>
	private void _OnFunctionChanged()
	{
		// if (functionPalette == null) return;
		// Function func = functionPalette.CurrentSelectedFunction;
        // IFunctionAST function = func.FunctionAST!;
		// object[] exampleEvaluations = new object[Math.Abs(func.EndTime - func.StartTime) + 1];
		// for (int i = func.StartTime; i <= func.EndTime; i++)
		// 	exampleEvaluations[i - func.StartTime] = function.EvaluateAtT(i);
		// GD.Print("Example Evaluation In Range: ");
		// GD.PrintS(exampleEvaluations);
	}

	/// <summary>
	/// Because of the line in _Ready, called whenever the selected
	/// function is being dragged.
	/// </summary>
	/// <param name="position">The global position of the function being dragged.</param>
	private void _OnFunctionDragging(Vector2 position)
	{
		if (functionPalette == null) return;
	}

	/// <summary>
	/// Because of the line in _Ready, called whenever the selected
	/// function has been dragged.
	/// </summary>
	/// <param name="position">The global position of the function that has been dragged.</param>
	private void _OnFunctionDragged(Vector2 position)
	{
		if (functionPalette == null) return;
	}
}
