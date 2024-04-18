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

    public override void _Ready()
    {
    }

    private void OnPlayButtonPressed()
	{
		EmitSignal(SignalName.PlayButtonPressed);
		GD.Print("\nPlay");
	}

	private void OnLoadButtonPressed()
	{
		GD.Print("Load");
		EmitSignal(SignalName.LoadButtonPressed);
	}

	private void OnSaveButtonPressed()
	{
		GD.Print("Save");
		EmitSignal(SignalName.SaveButtonPressed);
	}

	private void OnBeginningButtonPressed()
	{
		GD.Print("Beginning");
	}

	private void OnEndingButtonPressed()
	{
		GD.Print("Ending");
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
