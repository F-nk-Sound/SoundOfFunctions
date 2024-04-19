using Godot;
using System;
using System.Collections.Generic;

public partial class SaveSelector : Window
{
	public List<string> Saves { get; set; }

	[Export]
	ItemList? list;

	[Signal]
	public delegate void SelectedSaveEventHandler(string save);

	public void OnSelectSave(int index)
	{
		string save = Saves[index];
		Hide();

		EmitSignal(SignalName.SelectedSave, save);
	}

	public void OnClose()
	{
		Hide();
	}

	public void ShowSaves()
	{
		list!.Clear();
		foreach (string item in Saves)
			list.AddItem(item);
	}
}
