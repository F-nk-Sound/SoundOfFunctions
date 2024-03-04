using Functions;
using Godot;
using System;

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
		functionPalette.C_SelectedFunctionChanged += _OnFunctionChanged;
	}

	// Example signal capture
	private void _OnFunctionChanged(IFunctionAST functionAST)
	{
		if (functionPalette == null) return;
		IFunctionAST function = functionPalette.CurrentSelectedFunction;
		object[] exampleEvaluations = new object[100];
		for (int i = 0; i < exampleEvaluations.Length; i++)
			exampleEvaluations[i] = function.EvaluateAtT(i);
		GD.Print("Example Evaluation 1-100: ");
		GD.PrintS(exampleEvaluations);
	}
}
