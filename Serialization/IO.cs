using Godot;
using System.ComponentModel;
using System.Linq;

namespace Serialization;

/// <summary>
/// Reads and acts on all user input that has to do with Data Serialization.
/// </summary>
public partial class IO : Node
{
	/// <summary>
	/// Popup menu that holds the 3 possible save options outlined in <c>enum SerializationMode</c>.
	/// </summary>
	private PopupMenu? saveMenu;

	/// <summary>
	/// Tiny dialog menu that confirms if a user meant to make the save choice they chose.
	/// </summary>
	private ConfirmationDialog? saveConfirmation;

	/// <summary>
	/// Popup called when retrieving from the user the name under which to store their file.
	/// </summary>
	private Window? fileNameWindow;

	[Export]
	private SaveSelector? saveSelector;

	/// <summary>
	///  Holds the path where the file is to be loaded from/stored to.
	/// </summary>
	public string DataPath
	{
		get { return (_dataPath != null) ? _dataPath : ""; }
		set { _dataPath = ProjectSettings.GlobalizePath(value); }
	}
	private string? _dataPath;

	/// <summary>
	/// Holds the load/save mode of the serializer. Options are elaborated in <c>enum SerializationMode</c>.
	/// </summary>
	private int serializationMode;

	/// <summary>
	/// Specifies all possible load/save modes of the serializer.
	/// </summary>
	public enum SerializationMode
	{
		[Description("Saving both Timeline and Function Palette.")] COMPLETE_DECK,
		[Description("Saving only Timeline.")] TIMELINE,
		[Description("Saving only Function Palette.")] FUNCTION_PALETTE
	}

	/// <summary>
	/// Godot signal thrown when the user has provided all relevant information for serialization.
	/// </summary>
	/// <param name="state">The part of the application being serialized. See <c>enum SerializationMode</c> for more.</param>
	/// <param name="path">Location of file to create under which information will be serialized.</param>
	[Signal]
	public delegate void ReadyToSerializeEventHandler(int state, string path);

	/// <summary>
	/// Godot signal thrown when the user has provided all relevant information for deserialization.
	/// </summary>
	/// <param name="state">The part of the application being serialized. See <c>enum SerializationMode</c> for more.</param>
	/// <param name="path">Locaition of file to load from which information will be deserialized.</param>
	[Signal]
	public delegate void ReadyToDeSerializeEventHandler(int state, string path);

	public override void _Ready()
	{

		// Get children.
		saveSelector!.Hide();
		saveMenu = GetNode<PopupMenu>("SaveMenu");
		saveConfirmation = GetNode<ConfirmationDialog>("SaveMenu/SaveConfirmation");
		fileNameWindow = GetNode<Window>("FileNameWindow");

		// Set up file explorer.
		var explorerDirectory = ProjectSettings.GlobalizePath("user://") + "saves";
		if (!FileAccess.FileExists(explorerDirectory)) DirAccess.MakeDirAbsolute(explorerDirectory);

		fileNameWindow.GetChild<LineEdit>(0).TextSubmitted += OnSaveFileNameEntered;
		fileNameWindow.CloseRequested += OnFileNameWindowExitButtonPressed;
		saveConfirmation.Confirmed += OnSaveConfirmed;
		saveConfirmation.Canceled += OnSaveCanceled;
		saveMenu.IndexPressed += OnSaveMenuIndexPressed;
		GetNode<Serializer>("Serializer").ComponentSerialized += OnComponentSerialized;
		GetNode<Serializer>("Serializer").ComponentDeSerialized += OnComponentDeSerialized;
	}

	/// <summary>
	/// Handles the event where it has been indicated that the user wishes the save some part of the application.
	/// </summary>
	public void OnToolbarSaveButtonPressed()
	{
		if (saveMenu != null) if (!saveMenu.Visible) saveMenu.Show();
	}

	/// <summary>
	/// Handles the event where it has been indicated that the user wishes to load from a save file.
	/// </summary>
	public void OnToolBarLoadButtonPressed()
	{
		saveSelector!.Saves = DirAccess.GetFilesAt("user://saves").ToList();
		Vector2I viewportSize = (Vector2I)GetViewport().GetVisibleRect().Size;
		saveSelector.Position = viewportSize / 2 - saveSelector.Size / 2;
		saveSelector.ShowSaves();
		saveSelector.Show();
	}

	/// <summary>
	/// Handles the event where the user has selected the file to load from the SaveSelector.
	/// </summary>
	private void OnSaveSelected(string save)
	{
		// Grab the file extension.and globalized file path.
		// string extension = globalizedPath.Split('.')[1];

		// Set the serializaion mode accordingly.
		// if(extension.Equals("sfu")) serializationMode = (int) SerializationMode.COMPLETE_DECK;
		// else if (extension.Equals("sftl")) serializationMode = (int) SerializationMode.TIMELINE;
		// else if(extension.Equals("sfp")) serializationMode = (int) SerializationMode.FUNCTION_PALETTE;

		// Unsuported file extension selected.
		// else {
		// 	GetNode<AcceptDialog>("ErrorPopup").Title = "Error!";
		//     GetNode<AcceptDialog>("ErrorPopup").DialogText = "Unable to open file. Please Choose appropriate file format (.sfu, .sftl, .sfp).";
		//     GetNode<AcceptDialog>("ErrorPopup").Visible = true;
		// 	return;
		// }

		// Commence Deserialization.
		EmitSignal(SignalName.ReadyToDeSerialize, serializationMode, $"user://saves/{save}");
	}

	/// <summary>
	/// Handles the event where a save file name was succesfully inputted.
	/// </summary>
	/// <param name="fileName">Name of the file to save component to.</param>
	private void OnSaveFileNameEntered(string fileName)
	{

		// Get the globalized path and identify the serialization mode.
		string fullPath = ProjectSettings.GlobalizePath("user://") + "saves/" + fileName;
		fullPath += serializationMode switch
		{
			(int)SerializationMode.COMPLETE_DECK => ".sfu",     // .sfu -> Sound of Functions Unified
			(int)SerializationMode.TIMELINE => ".sftl",     // .sftl -> Sound of Functions Timeline
			(int)SerializationMode.FUNCTION_PALETTE => ".sfp",  // .sfp -> Sound of Functions Palette
			_ => ""
		};

		// Ensure Filename is unique so as to prevent overwriting.
		if (FileAccess.FileExists(fullPath))
		{
			GetNode<AcceptDialog>("ErrorPopup").Title = "Error!";
			GetNode<AcceptDialog>("ErrorPopup").DialogText = "File of this name already exisits. Please enter a valid file name.";
			GetNode<AcceptDialog>("ErrorPopup").Visible = true;
		} // Commence serialization.
		else
		{
			fileNameWindow?.Hide();
			DataPath = fullPath;
			EmitSignal(SignalName.ReadyToSerialize, serializationMode, fullPath);
		}
	}

	/// <summary>
	/// Handles the event where the exit button on the fileNameWindow as pressed.
	/// </summary>
	private void OnFileNameWindowExitButtonPressed()
	{
		fileNameWindow?.Hide();
	}

	/// <summary>
	/// Handles the event where the user has confirmed their wish to save part of the application.
	/// </summary>
	private void OnSaveConfirmed()
	{
		// Close Save Select and Save Confirm.
		saveConfirmation?.Hide();
		saveMenu?.Hide();
		for (int i = 0; i < saveMenu?.ItemCount; i++) saveMenu?.SetItemChecked(i, false);

		// Open file name window to retrieve what the user wants to save their file as.
		if (fileNameWindow != null)
		{
			fileNameWindow.Title = "Save " + saveMenu?.GetItemText(serializationMode);
			fileNameWindow.Show();
		}
	}

	/// <summary>
	/// Handles the event where the user has indicated they do not wish to keep their Save selection..
	/// </summary>
	private void OnSaveCanceled()
	{
		saveConfirmation?.Hide();
	}

	/// <summary>
	/// Handles the event where the user selects a save option by choosing one of the buttons on the menu.
	/// </summary>
	/// <param name="index">Save type indicated by the index. See <c>enum SerializationMode</c>.</param>
	private void OnSaveMenuIndexPressed(long index)
	{
		// Grabs serialiation mode based on user button input and toggles the rest of them.
		serializationMode = (int)index;
		for (int i = 0; i < saveMenu?.ItemCount; i++) saveMenu?.SetItemChecked(i, false);
		saveMenu?.ToggleItemChecked((int)index);

		// Ensure the user meant to choose the indicated serialization mode.
		if (saveConfirmation != null)
		{
			saveConfirmation.DialogText = "Save " + saveMenu?.GetItemText((int)index) + "?";
			saveConfirmation.Show();
		}
	}

	/// <summary>
	/// Handles the event where the serializer has indicated that a component has been sucessfuly serialized.
	/// </summary>
	/// <param name="path">Path to where the component is stored.</param>
	private void OnComponentSerialized(string path)
	{
		// Display save confirmation pop up for user.
		string fileName = path.Split("/").Last();
		AcceptDialog confimationWindow = GetNode<AcceptDialog>("ErrorPopup");
		string component = serializationMode switch
		{
			(int)SerializationMode.COMPLETE_DECK => "Deck ",
			(int)SerializationMode.TIMELINE => "Timeline ",
			(int)SerializationMode.FUNCTION_PALETTE => "Function Palette",
			_ => ""
		};
		confimationWindow.Title = "File Saved!";
		confimationWindow.DialogText = component + "sucessfully saved as: " + fileName;
		confimationWindow.Visible = true;
	}

	/// <summary>
	/// Handles the event where the serializer has indicated that a component has been sucessfuly serialized.
	/// </summary>
	/// <param name="path">Path to where the component is stored.</param>
	private void OnComponentDeSerialized(string path)
	{
		// Display save confirmation pop up for user.
		string fileName = path.Split("/").Last();
		AcceptDialog confimationWindow = GetNode<AcceptDialog>("ErrorPopup");
		confimationWindow.Title = "File Loaded!";
		confimationWindow.DialogText = fileName + " succesfully loaded.";
		confimationWindow.Visible = true;
	}
}
