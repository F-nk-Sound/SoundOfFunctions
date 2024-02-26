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
	FunctionPalette? functionPalette;

	Vector2 margins = new Vector2(20.0f, 10.0f);
	Vector2 minimumSize = new Vector2(225, 60);

	private void make()
	{
		if (latex == null || text == null || control == null) return;
		latex.Render();
		Vector2 size;
		size = new Vector2(Math.Max(latex.Width + margins.X, minimumSize.X), minimumSize.Y);
		text.CustomMinimumSize = size;
		control.CustomMinimumSize = new Vector2(Math.Max(text.Size.X, size.X), size.Y);
		latex.Position = new Vector2(size.X / 2, size.Y / 2);
		latex.Render();	
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		//functionPalette = GetNode<FunctionPalette>("UI/Function Palette");
		functionPalette = GetTree().CurrentScene.GetNode<FunctionPalette>("Function Palette");

		lsc = this.GetParent<ScrollContainer>();
		lsc.CustomMinimumSize = minimumSize;
		lsc.Size = minimumSize;
		control = this;
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
		make();
	}

	private void _OnTextChanged(String newText)
	{
		Vector2 size = new Vector2(Math.Max(latex.Width, minimumSize.X), minimumSize.Y);
		control.CustomMinimumSize = new Vector2(Math.Max(text.Size.X, size.X), size.Y);
	}

	private void _LineEditSubmitted(String finalText)
	{
		if (latex == null || text == null) return;
		if (finalText.IsEmpty())
			latex.LatexExpression = text.PlaceholderText;
		else
			latex.LatexExpression = finalText;
		lsc.GetHScrollBar().Value = lsc.GetHScrollBar().MinValue;
		make();
		text.ReleaseFocus();
		
		String functionText = text.Text;
		ParseResult result = Bridge.Parse(functionText);
		IFunctionAST ast = result.Unwrap();

		functionPalette.CurrentSelectedFunction = ast;
		functionPalette.EmitSignal(FunctionPalette.SignalName.SelectedFunctionChanged, functionPalette);
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
