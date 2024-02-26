using Functions;
using Godot;
using System;

public partial class Example : Node
{
    public override void _Ready()
    {
        base._Ready();
        FunctionPalette functionPalette = GetTree().CurrentScene.GetNode<FunctionPalette>("Function Palette");
        functionPalette.SelectedFunctionChanged += _OnFunctionChanged;
    }

    // Testing created signal
    private void _OnFunctionChanged(FunctionPalette functionPalette)
	{
		IFunctionAST function = functionPalette.CurrentSelectedFunction;
        object[] exampleEvaluations = new object[100];
        for (int i = 0; i < exampleEvaluations.Length; i++)
            exampleEvaluations[i] = function.EvaluateAtT(i);
        GD.Print("Example Evaluation 1-100: ");
        GD.PrintS(exampleEvaluations);
	}
}
