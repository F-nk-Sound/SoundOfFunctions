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
	private Godot.Range? _range;
	public Godot.Range? Range
	{
		get
		{
			return _range;
		}
		set
		{
			_range = value;
		}
	}
	private FunctionContainer? _functionContainerCopy;

	[Export]
	private Endpoint? _start;
	[Export]
	private Endpoint? _end;

	private bool _holding = false;
	private bool _dragging = false;
	public bool _isCopy = false;

	public override void _Ready()
	{
		RangeUpdate();

		if (_isCopy)
			return;
	}

	public void RangeUpdate()
	{
		if (_start == null || _end == null)
			return;

        Godot.Range temp = new()
        {
            MinValue = _start.Value,
            MaxValue = _end.Value
        };
        Range = temp;
	}

	/// <summary>
	/// Called when any text is inputted into the text box.
	/// </summary>
	/// <param name="event"></param>
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (FunctionPalette == null) return;

		if (@event is InputEventMouseButton mouseEvent)
		{
			
			if (_holding && _dragging && !mouseEvent.Pressed)
			{
				_holding = false;
				_dragging = false;
				FunctionPalette.OnDraggedEvent(mouseEvent.Position);
				FunctionPalette.RemoveChild(_functionContainerCopy);
			}

			if (mouseEvent.Position.X <= GlobalPosition.X
				|| mouseEvent.Position.X >= GlobalPosition.X + Size.X
				|| mouseEvent.Position.Y <= GlobalPosition.Y
				|| mouseEvent.Position.Y >= GlobalPosition.Y + Size.Y)
				return;

			if (!_holding && mouseEvent.Pressed)
			{
				_holding = true;
			}

			if (_holding && mouseEvent.IsReleased())
			{
				_holding = false;
			}
		}
		else
		{
			if (@event is InputEventMouseMotion motionEvent && _holding)
			{
				if ((motionEvent.Position.X <= GlobalPosition.X
				|| motionEvent.Position.X >= GlobalPosition.X + Size.X
				|| motionEvent.Position.Y <= GlobalPosition.Y
				|| motionEvent.Position.Y >= GlobalPosition.Y + Size.Y)
				&& !_dragging)
				{
					_functionContainerCopy = (FunctionContainer)Duplicate();
					_functionContainerCopy._isCopy = true;
					_functionContainerCopy.Position = motionEvent.Position - Size / 2;
					FunctionPalette.AddChild(_functionContainerCopy);
					_dragging = true;
				}
				if (_dragging && _functionContainerCopy != null)
				{
					FunctionPalette.OnDraggingEvent(motionEvent.Position);
					_functionContainerCopy.Position = motionEvent.Position - Size / 2;
				}
			}
		}
	}

	public void GraphFunction()
	{
		FunctionPalette!.GraphFunction(Function);
	}
}
