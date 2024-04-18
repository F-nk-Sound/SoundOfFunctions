using CSharpMath;
using Functions;
using Godot;
using Parsing;
using Sonification;

namespace UI.Palette;

/// <summary>
/// Updates the LineEdit/Text box inside the Function Palette,
/// along with updating the function palette script itself.
/// </summary>
public partial class TextUpdate : Control
{
	[Export]
	FunctionContainer? functionContainer;
	LaTeXButton? latex;
	LineEdit? text;
	
	public override void _Ready()
	{
		base._Ready();

		latex = GetChild<LaTeXButton>(0);
		text = GetChild<LineEdit>(1);

		latex.LatexExpression = text.PlaceholderText;
	}

	/// <summary>
	/// Called when the text box is "submitted"--when "Enter" is pressed.
	/// Sets the function palettes' current selected function.
	/// </summary>
	/// <param name="finalText">The entered text.</param>
	private void LineEditSubmitted(string finalText)
	{
		IFunctionAST? ast = null;
		Function? function = null;
		if(!text!.Text.IsEmpty())
		{
			ParseResult result = Bridge.Parse(finalText);
			ast = result.Unwrap();
			function = new Function(finalText, ast);
		}

		if (finalText.IsEmpty() || ast == null)
			latex!.LatexExpression = text.PlaceholderText;
		else
			latex!.LatexExpression = ast.Latex;
		text.ReleaseFocus();

		latex.Render();

		functionContainer!.Function = function!;
	}
	
	/// <summary>
	/// Called when the box is in focus--is selected. This does
	/// not set the function palettes' current selected function,
	/// see _LineEditSubmitted.
	/// </summary>
	private void OnFocusEntered()
	{
		text!.RemoveThemeColorOverride("font_color");
		latex!.Hide();
	}

	/// <summary>
	/// Called when the box is out of focus--is not selected.
	/// This does set the function palettes' current selected function,
	/// currently a design decision that can be modified.
	/// </summary>
	private void OnFocusExited()
	{
		text!.AddThemeColorOverride("font_color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
		latex!.Show();

		string functionText = text.Text;
		LineEditSubmitted(functionText);
	}
}
