using CSharpMath;
using Functions;
using Godot;
using Parsing;
using System;

/// <summary>
/// Updates the LineEdit/Text box inside the Function Palette,
/// along with updating the function palette script itself.
/// </summary>
public partial class TextUpdate : Control
{
	ScrollContainer? lsc;
	LaTeX? latex;
	LineEdit? text;
	Control? control;
	FunctionPalette? functionPalette;
	TextUpdate? textUpdateCopy;

	Vector2 margins = new Vector2(20.0f, 10.0f);
	Vector2 minimumSize = new Vector2(225, 80);

	private bool _dragging = false;
	private bool _moving = false;
	public bool _isCopy = false;

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

		functionPalette = GetTree().CurrentScene.GetNode<FunctionPalette>("Function Palette");

		if (_isCopy)
			return;

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
		if (latex == null || text == null || lsc == null || functionPalette == null) return;
		
		IFunctionAST? ast = null;
		if(!text.Text.IsEmpty())
		{
			ParseResult result = Bridge.Parse(finalText);
			ast = result.Unwrap();
		}

		if (finalText.IsEmpty() || ast == null)
			latex.LatexExpression = text.PlaceholderText;
		else
			latex.LatexExpression = ast.Latex;
		lsc.GetHScrollBar().Value = lsc.GetHScrollBar().MinValue;
		make();
		text.ReleaseFocus();

		functionPalette.CurrentSelectedFunction = ast;
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

	/// <summary>
	/// Called when any text is inputted into the text box.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (functionPalette == null) return;

		if (@event is InputEventMouseButton mouseEvent)
		{
			
			if (_dragging && _moving && !mouseEvent.Pressed)
			{
				_dragging = false;
				_moving = false;
				functionPalette.OnDraggedEvent(mouseEvent.Position);
				functionPalette.RemoveChild(textUpdateCopy);
			}

			if (mouseEvent.Position.X <= GlobalPosition.X
				|| mouseEvent.Position.X >= GlobalPosition.X + Size.X
				|| mouseEvent.Position.Y <= GlobalPosition.Y
				|| mouseEvent.Position.Y >= GlobalPosition.Y + Size.Y)
				return;

			if (!_dragging && mouseEvent.Pressed)
			{
				_dragging = true;
			}

			if (_dragging && mouseEvent.IsReleased())
			{
				_dragging = false;
			}
		}
		else
		{
			if (@event is InputEventMouseMotion motionEvent && _dragging)
			{
				if ((motionEvent.Position.X <= GlobalPosition.X
				|| motionEvent.Position.X >= GlobalPosition.X + Size.X
				|| motionEvent.Position.Y <= GlobalPosition.Y
				|| motionEvent.Position.Y >= GlobalPosition.Y + Size.Y)
				&& !_moving)
				{
					textUpdateCopy = (TextUpdate)Duplicate();
					textUpdateCopy._isCopy = true;
					textUpdateCopy.Position = motionEvent.Position - Size / 2;
					functionPalette.AddChild(textUpdateCopy);
					_moving = true;
				}
				if (_moving && textUpdateCopy != null)
				{
					functionPalette.OnDraggingEvent(motionEvent.Position);
					textUpdateCopy.Position = motionEvent.Position - Size / 2;
				}
			}
		}
	}

	/// <summary>
	/// Called when a function is closed.
	/// </summary>
	private void _OnExit()
	{
		ScrollContainer scrollContainer = GetParent<ScrollContainer>();
		VBoxContainer vBoxContainer = scrollContainer.GetParent<VBoxContainer>();
		vBoxContainer.RemoveChild(scrollContainer);
	}

}
