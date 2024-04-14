
using Godot;
using System;
using System.IO;
using UI;

namespace Serialization;

/// <summary>
/// Reads and acts on all user input that has to do with Data Serialization.
/// </summary>
public partial class IO : Node {

    /// <summary>
    /// FileDialog node to allow the user to interact with the application's save files.
    /// </summary>
    private FileDialog? fileExplorer;

    /// <summary>
    /// Popup menu that holds all the possible save options.
    /// </summary>
    private PopupMenu? saveMenu;

    /// <summary>
    /// Tiny dialog menu that confirms if a user meant to save.
    /// </summary>
    private ConfirmationDialog? saveConfirmation;

    /// <summary>
    /// Popup called when retrieving from the user the name under which to store their file.
    /// </summary>
    private Popup? fileNamePopup;

    /// <summary>
    ///  Holds the path where the file is to be read/stored.
    /// </summary>
    public string DataPath {
        get { return _dataPath; }
        set { _dataPath = ProjectSettings.GlobalizePath(value); }
    }
    private string _dataPath;
    
    /// <summary>
    /// Holds the save mode of the serializer. (0 = complete deck, 1 = timeline, 2 = function palette).
    /// </summary>
    private int saveMode;

    /// <summary>
    /// Godot signal thrown when the user has provided all relevant information for serialization.
    /// </summary>
    /// <param name="state">The part of the application being serialized. (0 = both, 1 = timeline, 2 = function palette)</param>
    /// <param name="path">File to create under which information will be serialzied.</param>
    [Signal]
    public delegate void ReadyToSerializeEventHandler(int state, string path);

    /// <summary>
    /// Godot signal thrown when the user has provided all relevant information for deserialization.
    /// </summary>
    /// <param name="path">File where information will be deserialzied from.</param>
    [Signal]
    public delegate void ReadyToDeSerializeEventHandler(string path);

    public override void _Ready() {
        
        // Get children.
        fileExplorer = GetNode<FileDialog>("FileExplorer");
        saveMenu = GetNode<PopupMenu>("SaveMenu");
        saveConfirmation = GetNode<ConfirmationDialog>("SaveMenu/SaveConfirmation");
        fileNamePopup = GetNode<Popup>("FileNamePopup");

        // Set up file explorer.
        var explorerDirectory = ProjectSettings.GlobalizePath("user://") + "saves";
        if(!Godot.FileAccess.FileExists(explorerDirectory)) DirAccess.MakeDirAbsolute(explorerDirectory);
        fileExplorer.CurrentDir = explorerDirectory;
        fileExplorer.RootSubfolder = explorerDirectory;
        fileExplorer.Hide();

        // Connect to necessary signals.
        GetTree().CurrentScene.GetNode<Toolbar>("Toolbar").SaveButtonPressed += OnToolbarSaveButtonPressed;
		GetTree().CurrentScene.GetNode<Toolbar>("Toolbar").LoadButtonPressed += OnToolBarLoadButtonPressed;
        fileExplorer.FileSelected += OnFileSelected;
        saveMenu.IndexPressed += OnSaveMenuIndexPressed;
        saveConfirmation.Confirmed += OnSaveConfirmed;
        saveConfirmation.Canceled += OnSaveCanceled;
        fileNamePopup.GetChild<LineEdit>(0).TextSubmitted += OnSaveFileNameEntered;
    }

    /// <summary>
    /// Handles the event where it has been indicated that the applications state should be saved.
    /// </summary>
    public void OnToolbarSaveButtonPressed() {
		saveMenu?.Show();
	}

	/// <summary>
	/// Handles the event where it has been indicated that the applications state should loaded from a save file.
	/// </summary>
	public void OnToolBarLoadButtonPressed() {
		//string path = ProjectSettings.GlobalizePath("user://savefile.json");
		//Deserialze(path);
        EmitSignal(SignalName.ReadyToDeSerialize, DataPath);
	}

    /// <summary>
    /// File to load from selected.
    /// </summary>
    /// <param name="path"></param>
    private void OnFileSelected(string path) {

    }

    /// <summary>
    /// Save menu button pressed.
    /// </summary>
    /// <param name="index">Save type.</param>
    private void OnSaveMenuIndexPressed(long index) {
        saveMode = (int) index;
        for(int i = 0; i < saveMenu?.ItemCount; i++) saveMenu?.SetItemChecked(i, false);
        saveMenu?.ToggleItemChecked((int) index);

        if(saveConfirmation != null) {
            saveConfirmation.DialogText = "Save " + saveMenu?.GetItemText((int)index) + "?";
            saveConfirmation.Show();
        }
    }

    /// <summary>
    /// Save type confirmed.
    /// </summary>
    private void OnSaveConfirmed() {
        // Close Save Select and Save Confirm.
        saveConfirmation?.Hide();
        saveMenu?.Hide();
        for(int i = 0; i < saveMenu?.ItemCount; i++) saveMenu?.SetItemChecked(i, false);

        // Open file name popop.
        if(fileNamePopup != null) {
            fileNamePopup.Title = "Save " + saveMenu?.GetItemText(saveMode);
            fileNamePopup.Show();
        }

    }

    /// <summary>
    /// Save type not confirmed.
    /// </summary>
    private void OnSaveCanceled() {
        saveConfirmation?.Hide();
    }

    /// <summary>
    /// Save file name succesfully inputted.
    /// </summary>
    /// <param name="fileName">Name of save file.</param>
    private void OnSaveFileNameEntered(string fileName) {
        string fullPath = ProjectSettings.GlobalizePath("user://") + "saves/" + fileName + ".json";
        if(Godot.FileAccess.FileExists(fullPath)) {
            if(fileNamePopup != null) fileNamePopup.GetChild<AcceptDialog>(1).Visible = true;
            return;
        } 
        fileNamePopup?.Hide();
        DataPath = fullPath;
        EmitSignal(SignalName.ReadyToSerialize, saveMode, fullPath);
    }

    /// <summary>
    /// Honestly no idea what this was here for right now, too tired while commenting.
    /// </summary>
    private void readPath() {

    }

}