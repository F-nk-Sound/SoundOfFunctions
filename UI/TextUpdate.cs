using CSharpMath;
using CSharpMath.Structures;
using Functions;
using Godot;
using Parsing;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

public partial class TextUpdate : Control
{
	ScrollContainer? lsc;
	LaTeX? latex;
	LineEdit? text;
	Control? control;

	private void make()
	{
		if (latex == null || text == null || lsc == null) return;
		latex.Render();
		Vector2 size;
		size = new Vector2(Math.Max(this.Position.X / 2 + latex.Width / 2, 225), Math.Max(this.Position.Y / 2 + latex.Height / 2, 60));
		text.Size = size;
		lsc.Size = new Vector2(225, 60);
		latex.Position = new Vector2(latex.Width / 2, size.Y / 2);
		latex.Render();	
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		lsc = this.GetChild<ScrollContainer>(0);
		lsc.CustomMinimumSize = new Vector2(225, 60);
		control = lsc.GetChild<Control>(0);
		latex = control.GetChild<LaTeX>(0);
		text = control.GetChild<LineEdit>(1);

		if (latex == null)
		{
			GD.PushError("Latex not initialized");
			return;
		}
		if (text == null)
		{
			GD.PushError("Text not initialized");
			return;
		}

		latex.LatexExpression = text.PlaceholderText;
	}

	private void _LineEditSubmitted(String finalText)
	{
		if (latex == null || text == null) return;
		GD.Print(finalText);
		if (finalText.IsEmpty())
			latex.LatexExpression = text.PlaceholderText;
		else
			latex.LatexExpression = finalText;
		make();
		text.ReleaseFocus();
		
		String functionText = text.Text;
		ParseResult result = Bridge.Parse(functionText);
		IFunctionAST ast = result.Unwrap();



		object[] atT = new object[100];
		for (int i = 0; i < 100; i++)
			atT[i] = ast.EvaluateAtT(i);

		GD.PrintS(atT);
	}
	
	private void _OnFocusEntered()
	{
		text.RemoveThemeColorOverride("font_color");
		latex.ZIndex = 0;
	}

	private void _OnFocusExited()
	{
		text.AddThemeColorOverride("font_color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
		latex.ZIndex = 1;
	}

}
