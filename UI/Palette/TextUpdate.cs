using System;
using System.Runtime;
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
	[Export]
	RichTextLabel? errorLabel;
	[Export]
	LaTeXButton? latex;
	[Export]
	LineEdit? text;
	
	public override void _Ready()
	{
		base._Ready();

		OnFocusExited();
		latex.LatexExpression = text.PlaceholderText;
	}

	void ReportError(string error)
	{
		errorLabel!.Text = $"[color=red]Error: {error}[/color]";
		errorLabel.Visible = true;
	}

	static string HumanifyError(string error)
	{
		return error
			.Replace("EOF", "end of input")
			.Replace("found at", "found at character(s)")
			.Replace("r#\"-?[0-9]+(\\\\.[0-9]+)?\"#", "number")
			.Replace("r#\"[a-zA-Zα-ωΑ-Ω](_[a-zA-Zα-ωΑ-Ω0-9]+)?\"#", "variable");
	}

	/// <summary>
	/// Called when the text box is "submitted"--when "Enter" is pressed.
	/// Sets the function palettes' current selected function.
	/// </summary>
	/// <param name="finalText">The entered text.</param>
	private void LineEditSubmitted(string finalText)
	{
		IFunctionAST ast;
		Function? function = null;
		if (finalText.IsNonEmpty())
		{
			ParseResult result = Bridge.Parse(finalText);

			if (result is Success ok)
			{
				ast = ok.Value;
				errorLabel!.Visible = false;
			}
			else if (result is Failure err)
			{
				ReportError(HumanifyError(err.Error));
				return;
			}
			else 
			{
				throw new InvalidCastException("expected a result type");
			}

			if (ASTHasUnboundVariable(ast) is string unbound)
			{
				ReportError($"Unknown variable '{unbound}'");
				return;
			}

			function = new Function(finalText, ast, 0, 1);
			latex!.LatexExpression = ast.Latex;
		}
		else
		{
			latex!.LatexExpression = text!.PlaceholderText;
		}

        functionContainer!.Function = function!;

        if (IsNodeReady())
		{
            text!.ReleaseFocus();
            latex.Render();
            functionContainer.GraphFunction();
        }
	}

	public void ModifyText(string newText)
	{
		text!.Text = newText;
		LineEditSubmitted(newText);
	}

	/// <summary>
	/// Returns true if an unbound variable is found.
	/// </summary>
	/// <param name="ast"></param>
	/// <returns></returns>
	static string? ASTHasUnboundVariable(IFunctionAST ast)
	{
		return ast switch
        {
            Absolute val => ASTHasUnboundVariable(val.Inner),
            Add val => ASTHasUnboundVariable(val.Left) ?? ASTHasUnboundVariable(val.Right),
            Ceil val => ASTHasUnboundVariable(val.Inner),
            Cosine val => ASTHasUnboundVariable(val.Inner),
            Divide val => ASTHasUnboundVariable(val.Left) ?? ASTHasUnboundVariable(val.Right),
            Exponent val => ASTHasUnboundVariable(val.Base) ?? ASTHasUnboundVariable(val.Power),
            Floor val => ASTHasUnboundVariable(val.Inner),
            Multiply val => ASTHasUnboundVariable(val.Left) ?? ASTHasUnboundVariable(val.Right),
            Number => null,
            Sine val => ASTHasUnboundVariable(val.Inner),
            Subtract val => ASTHasUnboundVariable(val.Left) ?? ASTHasUnboundVariable(val.Right),
            Tangent val => ASTHasUnboundVariable(val.Inner),
            Variable val => val.Name != "t" ? val.Name : null,
            _ => throw new NotImplementedException(),
        };
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

		if (functionContainer!.Function is not null)
		{
			functionContainer.GraphFunction();
		}
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
