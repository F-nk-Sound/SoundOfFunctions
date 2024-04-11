using Godot;
using Sonification;
using System;

namespace UI;

public partial class Toolbar : Node2D {

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

	public override void _Ready() {
		
		// Connect all the button signals to the Toolbar.
		GetNode<Button>($"Toolbar BG/Play").Pressed += OnPlayButtonPressed;
		GetNode<Button>($"Toolbar BG/Exit").Pressed += OnExitButtonPressed;
		GetNode<Button>($"Toolbar BG/Load").Pressed += OnLoadButtonPressed;
		GetNode<Button>($"Toolbar BG/Save").Pressed += OnSaveButtonPressed;
		GetNode<Button>($"Toolbar BG/Beginning").Pressed += OnBeginningButtonPressed;
		GetNode<Button>($"Toolbar BG/Ending").Pressed += OnEndingButtonPressed;
		GetNode<Button>($"Toolbar BG/Redo").Pressed += OnRedoPressed;
		GetNode<Button>($"Toolbar BG/Undo").Pressed += OnUndoPressed;
		
	}
	
	private void OnPlayButtonPressed() {
		EmitSignal(SignalName.PlayButtonPressed);
		GD.Print("\nPlay");
	}

	private void OnExitButtonPressed() {
		GetTree().Quit();
	}

	private void OnLoadButtonPressed() {
		EmitSignal(SignalName.LoadButtonPressed);
		GD.Print("Load");
	}

	private void OnSaveButtonPressed() {
		EmitSignal(SignalName.SaveButtonPressed);
		GD.Print("Save");
	}

	private void OnBeginningButtonPressed() {
		GD.Print("Beginning");
	}

	private void OnEndingButtonPressed() {
		GD.Print("Ending");
	}

	private void OnRedoPressed() {
		GD.Print("Redo");
	}

	private void OnUndoPressed() {
		GD.Print("Undo");
	}

}
