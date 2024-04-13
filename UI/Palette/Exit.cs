using Godot;
using System;

public partial class Exit : Button
{
	public override void _Ready()
	{
		Pressed += OnExit;
	}

	public void OnExit()
	{
		Owner.GetParent<VBoxContainer>().RemoveChild(Owner);
	}
}
