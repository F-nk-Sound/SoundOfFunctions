
using Godot;
using System;
using System.IO;
using Sonification;
using Functions;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UI.Palette;
using UI;
using Godot.Collections;
using Parsing;

namespace Serialization;

/// <summary>
/// Data serialization for the Application.
/// </summary>
public partial class Serializer : Node {

	/// <summary>
	/// Current FunctionPalette Node of the application.
	/// </summary>
	private FunctionPalette? functionPalette;

	/// <summary>
	/// Current UpperTimeline Node of the application.
	/// </summary>
	private UpperTimeline? upperTimeline;

	/// <summary>
	/// Current LowerTimeline Node of the application.
	/// </summary>
	private LowerTimeline? lowerTimeline;

	/// <summary>
	/// Godot event called when a Function Palette has been loaded from Json.
	/// </summary>
	[Signal]
	public delegate void FunctionPaletteLoadedEventHandler(UpperTimeline lt);

	/// <summary>
	/// Godot event called when an Upper timeline has been loaded from Json.
	/// </summary>
	[Signal]
	public delegate void UpperTimelineLoadedEventHandler(UpperTimeline lt);

	/// <summary>
	/// Godot event called when a Lower timeline has been loaded from Json.
	/// </summary>
	[Signal]
	public delegate void LowerTimelineLoadedEventHandler(LowerTimeline lt);

	public override void _Ready() {

		// Grab relevant nodes.
		functionPalette = GetTree().CurrentScene.GetNode<FunctionPalette>("Function Palette");
		upperTimeline = GetTree().CurrentScene.GetNode<UpperTimeline>("Timeline");
		lowerTimeline = GetTree().CurrentScene.GetNode<AudioGenerator>("Timeline/AudioGenerator").timeline;

		// Grab relevant signals.
		((IO) GetParent()).ReadyToSerialize += OnReadyToSerialize; 
		((IO) GetParent()).ReadyToDeSerialize += OnReadyToDeSerialize; 

	}
	
	/// <summary>
	/// Handles the event where the program is raedy to serialize data.
	/// </summary>
	/// <param name="state">The part of the application being serialized. (0 = both, 1 = timeline, 2 = function palette)</param>
    /// <param name="path">File to create under which information will be serialzied.</param>
	private void OnReadyToSerialize(int state, string path) {

		string? json;
		switch (state) {
            // Saving both Function Palette and Timeline.
            case 0:
				json = SerializeSessionToJSON();
                break;

            // Saving Timeline.
            case 1:
				json = SerializeTimelineToJSON();
                break;

            // Saving Function Palette.
            case 2:
				json = SerializeFunctionPaletteToJSON();
                break;
			default:
				json = null;
				break;
        }
		File.WriteAllText(path, json);
	}

	/// <summary>
	/// Handles the event where the program is raedy to deserialize data.
	/// </summary>
    /// <param name="path">File where information will be deserialzied from.</param>
	private void OnReadyToDeSerialize(string path) {
		// Only works under right now with LowerTimeline.
		DeserializeJSONToSession(path);
	}

	/// <summary>
	/// Serializes the applications current state by turning all relevant infomration to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds all necessary information to load the application from said file.</returns>
	private string SerializeSessionToJSON() {

		var res = new Godot.Collections.Dictionary();

		// Serialize FunctionPalette.
		if(functionPalette != null) res.Add("FunctionPalette", functionPalette.Save());

		// Serialize UpperTimeline.
		if(upperTimeline != null) res.Add("UpperTimeline", upperTimeline.Save());

		// Serialize LowerTimeline.
		if(lowerTimeline != null) res.Add("LowerTimeline", lowerTimeline.Save());

		return Json.Stringify(res, "\t");
	}

	/// <summary>
	/// Serializes the Upper and Lower Timelines to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds the timelines.</returns>
	private string SerializeTimelineToJSON() {

		var res = new Godot.Collections.Dictionary();

		// Serialize UpperTimeline.
		if(upperTimeline != null) res.Add("UpperTimeline", upperTimeline.Save());

		// Serialize LowerTimeline.
		if(lowerTimeline != null) res.Add("LowerTimeline", lowerTimeline.Save());

		return Json.Stringify(res, "\t");

	}

	/// <summary>
	/// Serializes the Function Palette to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds the function palette.</returns>
	private string SerializeFunctionPaletteToJSON() {

		// Serialize FunctionPalette.
		if(functionPalette != null) {
			return Json.Stringify(functionPalette.Save(), "\t");
		}
		return "";

	}

	/// <summary>
	/// Deserializes a JSON which holds the information to load a previous save of the application.
	/// </summary>
	/// <param name="path">Path to the saved JSON file to load.</param>
	public void DeserializeJSONToSession(string path) {
		
		// Ensure File exists.
		if(!Godot.FileAccess.FileExists(path)) return;

		// Grab and read the Json file using Json Loader.
		string json = File.ReadAllText(path); 
		Json jsonLoader = new Json(); 
		Error errorReadingFile = jsonLoader.Parse(json);

		// Ensure file was properly parsed.
		if(errorReadingFile != Error.Ok) return;

		// Actually Load the data into new objects.
		var fileLoadedDict = (Dictionary) jsonLoader.Data;        // Holds Main Dictionary to Saved Components.

		// Grab each Node's base information.
		var lowerTimelineDictionary = fileLoadedDict["LowerTimeline"].AsGodotDictionary();  // LowerTimeline Dictionary.
		//var functionPaletteDict = fileLoadedDict["FunctionPalette"].AsGodotDictionary();    // FunctionPalette Dictionary.
		//var upperTimelineDict = fileLoadedDict["UpperTimeline"].AsGodotDictionary();        // UpperTimeline Dictionary.

		// Load lower timeline.
		LoadLowerTimeline(lowerTimelineDictionary);
	}   

	/// <summary>
	/// Loads a FunctionPalette instance into the application.
	/// </summary>
	/// <param name="functionPaletteDictionary">Dictionary holding information needed to create a LowerTimeline</param>
	private void LoadFunctionPalette(Dictionary functionPaletteDictionary) {

	}

	/// <summary>
	/// Loads an UpperTimeline instance into the application.
	/// </summary>
	/// <param name="upperTimelineDictionary">Dictionary holding information needed to create a LowerTimeline</param>
	private void LoadUpperTimeline(Dictionary upperTimelineDictionary) {

	}

	/// <summary>
	/// Loads a LowerTimeline instance into the application.
	/// </summary>
	/// <param name="lowerTimelineDictionary">Dictionary holding information needed to create a LowerTimeline</param>
	private void LoadLowerTimeline(Dictionary lowerTimelineDictionary) {
		// Grab all LowerTimeline information.
		var timelineCount = (int) lowerTimelineDictionary["Count"];
		var timelineFunctionDict = lowerTimelineDictionary["Functions"].AsGodotDictionary();
	
		// Iterate through functions and add to a new list.
		List<Function> newFunctionList = new List<Function>();
		for(int i = 0; i < timelineCount; i++) {
			// Get Function Information.
			var currFuncDict = timelineFunctionDict[i.ToString()].AsGodotDictionary();
			var endTime = (int) currFuncDict["EndTime"];
			var startTime = (int) currFuncDict["StartTime"];
			var textRepresentation = (string) currFuncDict["TextRepresentation"];
			
			// Create new Function.
			IFunctionAST newAST = Bridge.Parse(textRepresentation).Unwrap();
			Function newFunction = new Function(textRepresentation, newAST); 
			newFunction.StartTime = startTime;
			newFunction.EndTime = endTime;
		
			// Add Function to the list.
			newFunctionList.Add(newFunction);
		}

		// Create a new Timeline.
		LowerTimeline newTimeline = new LowerTimeline();
		newTimeline.Name = "Loaded Timeline";
		newTimeline.SetProcess(false);
		newTimeline.Add(newFunctionList);

		EmitSignal(SignalName.LowerTimelineLoaded, newTimeline);
	}

}
