using Godot;
using System;

namespace Graphing;

public partial class ExampleScene : VBoxContainer {
	private FunctionRenderer fr;
	private LineEdit le;
	public override void _Ready() {
		fr = GetNode<FunctionRenderer>("FunctionRenderer");
		le = GetNode<LineEdit>("HBoxContainer/LineEdit");
	}
	private void _on_button_pressed() {
	}
}


