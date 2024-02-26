using CSharpMath.Structures;
using Functions;
using Godot;
using Parsing;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public partial class TextUpdate : Control
{
	ScrollContainer? lsc;
	LaTeX? latex;
	LineEdit? text;
	double timeBetween = 0.75;
	double total = 0.0;
	bool active = true;

	private void make()
	{
		if (latex == null || text == null) return;
		latex.Render();
		Vector2 size;
		size = new Vector2(Math.Max(this.Position.X / 2 + latex.Width / 2, 225), Math.Max(this.Position.Y / 2 + latex.Height / 2, 60));
		text.Size = size;
		latex.Position = new Vector2(latex.Width / 2, size.Y / 2);
		GD.Print(latex.Position);
		latex.Render();	
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		lsc = this.GetChild<ScrollContainer>(0);
		lsc.CustomMinimumSize = new Vector2(225, 70);
		latex = lsc.GetChild<LaTeX>(0);
		text = this.GetChild<LineEdit>(1);

		if (latex == null)
			GD.PrintErr("Latex not initialized");
		if (text == null)
			GD.PrintErr("Text not initialized");

		this.OnTextChanged();
	}

	public override void _Process(double delta)
	{
		total += delta;
		if (total > timeBetween)
		{
			total = 0.0;
			active = !active;
		}
	}
	
	private void OnTextChanged()
	{
		if (latex == null || text == null) return;
		String newText = text.Text;
		if (newText.Equals(""))
			latex.LatexExpression = text.PlaceholderText;
		else
			latex.LatexExpression = newText;
	}

	private void OnTextInput(InputEvent @event)
	{
		if (latex == null || text == null) return;
		if (!(@event is InputEventKey keyEvent && keyEvent.Pressed)) return;
		if (keyEvent.Keycode != Key.Enter) return;
	}

	private void OnTextSet()
	{
		if (latex == null || text == null) return;
		GD.Print(text.Text);
	}

	private void _LineEditSubmitted(String finalText)
	{
		if (latex == null || text == null) return;
		GD.Print(finalText);
		make();
		text.ReleaseFocus();
		
		return;
		String functionText = text.Text;
		ParseResult result = Bridge.Parse(functionText);
		IFunctionAST ast = result.Unwrap();
		for (int i = 0; i < 100; i++)
			GD.PrintRaw(ast.EvaluateAtT(i).ToString() + " ");
	}
	
	private void _OnFocusEntered()
	{
		text.RemoveThemeColorOverride("font_color");
		latex.ZIndex = 1;
	}

	private void _OnFocusExited()
	{
		text.AddThemeColorOverride("font_color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
		latex.ZIndex = 0;
	}

}
