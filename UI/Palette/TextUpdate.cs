using CSharpMath;
using Functions;
using Godot;
using Parsing;
using Sonification;
using System;

namespace UI.Palette;

/// <summary>
/// Updates the LineEdit/Text box inside the Function Palette,
/// along with updating the function palette script itself.
/// </summary>
public partial class TextUpdate : Control
{
	FunctionContainer? functionContainer;
	ScrollContainer? lsc;
	LaTeX? latex;
	LineEdit? text;
	Control? control;
	TextUpdate? textUpdateCopy;

	Vector2 margins = new Vector2(20.0f, 10.0f);
	Vector2 minimumSize = new Vector2(225, 80);

	/// <summary>
	/// Adjusts the LaTeX node to fit the same space as the current text box.
	/// Also adjusts to fit within the minimumSize.
	/// </summary>
	private void make()
	{
		if (latex == null || text == null || control == null) return;

		latex.Render();
		Vector2 size;
		size = new Vector2(Math.Max(latex.Width + margins.X, minimumSize.X), minimumSize.Y);
		text.CustomMinimumSize = size;
		control.CustomMinimumSize = new Vector2(Math.Max(text.Size.X, size.X), size.Y);
		latex.Position = new Vector2(size.X / 2, size.Y / 2 + margins.Y);
	}
	
	public override void _Ready()
	{
		base._Ready();
		functionContainer = (FunctionContainer) Owner;

		lsc = GetParent<ScrollContainer>();
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
		if (latex == null || text == null || control == null) return;

		Vector2 size = new Vector2(Math.Max(latex.Width, minimumSize.X), minimumSize.Y);
		control.CustomMinimumSize = new Vector2(Math.Max(text.Size.X, size.X), size.Y);
	}

	/// <summary>
	/// Called when the text box is "submitted"--when "Enter" is pressed.
	/// Sets the function palettes' current selected function.
	/// </summary>
	/// <param name="finalText">The entered text.</param>
	private void _LineEditSubmitted(String finalText)
	{
		if (latex == null || text == null || lsc == null || functionContainer == null) return;
		
		IFunctionAST? ast = null;
		Function? function = null;
		if(!text.Text.IsEmpty())
		{
			ParseResult result = Bridge.Parse(finalText);
			ast = result.Unwrap();
			function = new Function(finalText, ast);
		}

		if (finalText.IsEmpty() || ast == null)
			latex.LatexExpression = text.PlaceholderText;
		else
			latex.LatexExpression = ast.Latex;
		lsc.GetHScrollBar().Value = lsc.GetHScrollBar().MinValue;
		make();
		text.ReleaseFocus();

		functionContainer.Function = function;
	}
	
	/// <summary>
	/// Called when the box is in focus--is selected. This does
	/// not set the function palettes' current selected function,
	/// see _LineEditSubmitted.
	/// </summary>
	private void _OnFocusEntered()
	{
		if (text == null || latex == null) return;

		text.RemoveThemeColorOverride("font_color");
		latex.ZIndex = 0;
	}

	/// <summary>
	/// Called when the box is out of focus--is not selected.
	/// This does set the function palettes' current selected function,
	/// currently a design decision that can be modified.
	/// </summary>
	private void _OnFocusExited()
	{
		if (text == null || latex == null) return;

		text.AddThemeColorOverride("font_color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
		latex.ZIndex = 1;

		String functionText = text.Text;
		_LineEditSubmitted(functionText);
	}
}
