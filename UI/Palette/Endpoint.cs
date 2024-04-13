using Godot;
using System;

namespace UI.Palette;
public partial class Endpoint : Node
{
	private const float MAX = 10000.0f;

	private SpinBox? _endpoint;
	private float _value;
	public float Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
            ((FunctionContainer)Owner).RangeUpdate();
        }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		_endpoint = GetChild<SpinBox>(1);
		_endpoint.ValueChanged += OnValueChanged;
		Value = (float) _endpoint.Value;
	}

	public void OnValueChanged(double value)
	{
		if (value >= MAX)
            _endpoint.Value = MAX;
		else if (value <= -MAX)
            _endpoint.Value = -MAX;

		Value = (float) value;
		_endpoint.ReleaseFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
