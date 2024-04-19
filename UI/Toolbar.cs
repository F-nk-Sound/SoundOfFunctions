using Godot;
using Sonification;
using System;

namespace UI;

public partial class Toolbar : Control
{	

	/// <summary>
	/// Godot signal emitted when the play button is pressed.
	/// </summary>
	[Signal]
	public delegate void PlayButtonPressedEventHandler();

	/// <summary>
	/// Godot signal emitted when the play button is pressed.
	/// </summary>
	[Signal]
	public delegate void SaveButtonPressedEventHandler();

	/// <summary>
	/// Godot signal emitted when the play button is pressed.
	/// </summary>
	[Signal]
	public delegate void LoadButtonPressedEventHandler();

	/// <summary>
	/// Godot signal emitted when the previous button is pressed.
	/// </summary>
	[Signal]
	public delegate void PreviousButtonPressedEventHandler();

	/// <summary>
	/// Godot signal emitted when the next button is pressed.
	/// </summary>
	[Signal]
	public delegate void NextButtonPressedEventHandler();

	public override void _Ready()
	{
	}

	private void OnPlayButtonPressed()
	{
		EmitSignal(SignalName.PlayButtonPressed);
	}

	private void OnLoadButtonPressed()
	{
		EmitSignal(SignalName.LoadButtonPressed);
	}

	private void OnSaveButtonPressed()
	{
		EmitSignal(SignalName.SaveButtonPressed);
	}

	private void OnPreviousButtonPressed()
	{	
		EmitSignal(SignalName.PreviousButtonPressed);
	}

	private void OnNextButtonPressed()
	{
		EmitSignal(SignalName.NextButtonPressed);
	}

	private void OnRedoPressed()
	{
		GD.Print("Redo");
	}

	private void OnUndoPressed()
	{
		GD.Print("Undo");
	}

}
