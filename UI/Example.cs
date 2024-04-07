using Functions;
using Godot;
using System;

using UI;

public partial class Example : Node
{
	private FunctionPalette? functionPalette;

	public override void _Ready()
	{
		base._Ready();

		// Get the Function Palette of the scene
		functionPalette = GetTree().CurrentScene.GetNode<FunctionPalette>("Function Palette");

		// Connect signal when the current selected function changes.
		// Assumptively to be used for the graph view.
		functionPalette.SelectedFunctionChanged += _OnFunctionChanged;

		// Connect signal when the current selected funciton is being dragged.
		functionPalette.FunctionDragging += _OnFunctionDragging;

		// Connect signal when the current selected funciton has been dragged.
		functionPalette.FunctionDragged += _OnFunctionDragged;
	}

	/// <summary>
	/// Because of the line in _Ready, called whenever the selected
	/// function is changed.
	/// </summary>
	private void _OnFunctionChanged()
	{
		if (functionPalette == null) return;
		IFunctionAST function = functionPalette.CurrentSelectedFunction.FunctionAST;
		object[] exampleEvaluations = new object[100];
		for (int i = 0; i < exampleEvaluations.Length; i++)
			exampleEvaluations[i] = function.EvaluateAtT(i);
		GD.Print("Example Evaluation 1-100: ");
		GD.PrintS(exampleEvaluations);
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
