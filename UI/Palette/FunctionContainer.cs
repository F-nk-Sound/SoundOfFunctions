using Godot;
using Sonification;
using System;

namespace UI.Palette;
public partial class FunctionContainer : Control
{
	public FunctionPalette? FunctionPalette;
	private Function? _function;
	public Function Function
	{
		get
		{
			return _function!;
		}
		set
		{
			_function = value;

			if (Range != null && _function != null)
			{
				_function.StartTime = (int)Range.MinValue;
				_function.EndTime = (int)Range.MaxValue;
			}

			if (FunctionPalette != null)
				FunctionPalette.CurrentSelectedFunction = value;
		}
	}

	public Godot.Range? Range { get; set; }
	private FunctionContainer? _functionContainerCopy;

	[Export]
	private SpinBox? _start;
	[Export]
	private SpinBox? _end;

	[Export]
	private TextUpdate? _textUpdate;

	private bool _holding = false;
	private bool _dragging = false;
	public bool _isCopy = false;

	public override void _Ready()
	{
		RangeUpdate();

		if (_isCopy)
			return;
	}

	public void FunctionUpdate(string TextRepresentation, float startTime, float endTime)
	{
		_textUpdate!.ModifyText(TextRepresentation);
		_start!.Value = startTime;
		_end!.Value = endTime;
		RangeUpdate();
	}

	public void RangeUpdate()
	{
		Godot.Range temp = new()
		{
			MinValue = _start!.Value,
			MaxValue = _end!.Value
		};
		Range = temp;

		_start.Value = Range.MinValue;
		_end.Value = Range.MaxValue;

		if (_function is not null)
		{
			_function.StartTime = (int)Range.MinValue;
			_function.EndTime = (int)Range.MaxValue;
		}
	}

	void RangeChanged(float value)
	{
		RangeUpdate();
	}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (Function is null) return default;

		Control display = (Control)Duplicate();
		SetDragPreview(display);
		Function newFunc = (Function)Function.Duplicate();
		newFunc.Initialize(Function.TextRepresentation!, Function.FunctionAST!, Function.StartTime, Function.EndTime);
		return newFunc;
	}

	public void GraphFunction()
	{
		FunctionPalette!.GraphFunction(Function);
	}
}
