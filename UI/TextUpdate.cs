using CSharpMath;
using CSharpMath.Structures;
using Functions;
using Godot;
using Parsing;
using System;
using System.Diagnostics;
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
	TextUpdate? textUpdateCopy;

	Vector2 margins = new Vector2(20.0f, 10.0f);
	Vector2 minimumSize = new Vector2(225, 60);

	private bool _dragging = false;
	private bool _moving = false;
	public bool _isCopy = false;

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

	private void _LineEditSubmitted(String finalText)
	{
		if (latex == null || text == null || lsc == null || functionPalette == null) return;
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
		functionPalette.EmitSignal(FunctionPalette.SignalName.SelectedFunctionChanged);
	}
	
	private void _OnFocusEntered()
	{
		if (text == null || latex == null) return;
		text.RemoveThemeColorOverride("font_color");
		latex.ZIndex = 0;
	}

	private void _OnFocusExited()
	{
		if (text == null || latex == null) return;
		text.AddThemeColorOverride("font_color", new Color(0.0f, 0.0f, 0.0f, 0.0f));
		latex.ZIndex = 1;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (functionPalette == null) return;
		if (@event is InputEventMouseButton mouseEvent)
		{
			
			if (_dragging && !mouseEvent.Pressed)
			{
				_dragging = false;
				_moving = false;
				functionPalette.EmitSignal(FunctionPalette.SignalName.FunctionDragged, mouseEvent.Position);
				functionPalette.RemoveChild(textUpdateCopy);
			}

			if (!(mouseEvent.Position.X > GlobalPosition.X)
				&& !(mouseEvent.Position.X < GlobalPosition.X + Size.X)
				&& !(mouseEvent.Position.Y > GlobalPosition.Y)
				&& !(mouseEvent.Position.Y < GlobalPosition.Y + Size.Y))
			{
				return;
			}

			if (!_dragging && mouseEvent.Pressed)
			{
				_dragging = true;
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
					functionPalette.EmitSignal(FunctionPalette.SignalName.FunctionDragging, motionEvent.Position);
					textUpdateCopy.Position = motionEvent.Position - Size / 2;
				}
			}
		}
	}

	private void _OnExit()
	{
		ScrollContainer scrollContainer = GetParent<ScrollContainer>();
		VBoxContainer vBoxContainer = scrollContainer.GetParent<VBoxContainer>();
		vBoxContainer.RemoveChild(scrollContainer);
	}

}
