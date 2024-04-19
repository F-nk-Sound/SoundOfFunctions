using Godot;
using System.IO;
using Sonification;
using System.Collections.Generic;
using UI.Palette;
using UI;
using Godot.Collections;
using Parsing;
using System.Linq;

namespace Serialization;

/// <summary>
/// Data serialization for the Application.
/// </summary>
public partial class Serializer : Node {

	/// <summary>
	/// Current FunctionPalette Node of the application.
	/// </summary>
	[Export]
	private FunctionPalette? functionPalette;

	/// <summary>
	/// Current UpperTimeline Node of the application.
	/// </summary>
	[Export]
	private UpperTimeline? upperTimeline;

	[Export]
	private AudioGenerator? audioGenerator;

	/// <summary>
	/// Current LowerTimeline Node of the application.
	/// </summary>
	[Export]
	private LowerTimeline? lowerTimeline;

	/// <summary>
	/// File name to be added as name of each component for identification purposes.
	/// </summary>
	private string? componentIdentifier;

	/// <summary>
	/// Godot event called when a component has been sucessfully serialized into the file system.
	/// </summary>
	/// <param name="path">Path to where the component is stored.</param>
	[Signal]
	public delegate void ComponentSerializedEventHandler(string path);

	/// <summary>
	/// Godot event called when a component has been sucessfully deserialized and is ready to be loaded into the program.
	/// </summary>
	/// <param name="path">Path to where the component is stored.</param>
	[Signal]
	public delegate void ComponentDeSerializedEventHandler(string path);

	/// <summary>
	/// Godot event called when a Function Palette Node has been loaded from Json.
	/// </summary>
	/// <param name="fp">FunctionPalette Node that was loaded from JSON.</param>
	[Signal]
	public delegate void FunctionPaletteLoadedEventHandler();

	/// <summary>
	/// Godot event called when an Upper Timeline Node has been loaded from Json.
	/// </summary>
	/// <param name="ut">UpperTimeline Node that was loaded from JSON.</param>
	[Signal]
	public delegate void UpperTimelineLoadedEventHandler();

	/// <summary>
	/// Godot event called when a Lower Timeline Node has been loaded from Json.
	/// </summary>
	/// <param name="ut">LowerTimeline Node that was loaded from JSON.</param>
	[Signal]
	public delegate void LowerTimelineLoadedEventHandler();

	public override void _Ready() {
		// Grab relevant signals.
		((IO) GetParent()).ReadyToSerialize += OnReadyToSerialize; 
		((IO) GetParent()).ReadyToDeSerialize += OnReadyToDeSerialize; 
	}
	
	/// <summary>
	/// Handles the event where the program is raedy to serialize data.
	/// </summary>
	/// <param name="state">Serializer save state. See <c>IO.SerializationMode</c> for more elaboration.</param>
	/// <param name="path">File to create under which information will be serialzied.</param>
	private void OnReadyToSerialize(int state, string path) {

		// Set the component identifier and serialize accordingly based on the serialization mode.
		componentIdentifier = path.GetBaseName().Split("/").Last();
		string? saveFile = state switch {
			(int)IO.SerializationMode.COMPLETE_DECK => SerializeCompleteDeckToJSON(),
			(int)IO.SerializationMode.TIMELINE => SerializeTimelineToJSON(),
			(int)IO.SerializationMode.FUNCTION_PALETTE => SerializeFunctionPaletteToJSON(),
			_ => null,
		};
		File.WriteAllText(path, saveFile);

		// Send out succesful serialization signal.
		EmitSignal(SignalName.ComponentSerialized, path);
	}

	/// <summary>
	/// Handles the event where the program is raedy to deserialize data.
	/// </summary>
	/// <param name="state">Serializer load state. See <c>IO.SerializationMode</c> for more elaboration.</param>
	/// <param name="path">File where information will be deserialized from.</param>
	private void OnReadyToDeSerialize(int state, string path) {

		// Ensure File exists.
		if(!Godot.FileAccess.FileExists(path)) return;

		// Read file using Json Loader, Ensure file was properly parsed.
		string json = Godot.FileAccess.GetFileAsString(path); 
		Json jsonLoader = new(); 
		Error errorReadingFile = jsonLoader.Parse(json);
		if(errorReadingFile != Error.Ok) return;

		// Begin file deserialization.
		var configData = (Dictionary) jsonLoader.Data;     
		switch (state) {
			case (int) IO.SerializationMode.COMPLETE_DECK:
				DeserializeJSONToCompleteDeck(configData);
				break;
			case (int) IO.SerializationMode.TIMELINE:
				DeserializeJSONToTimeline(configData);
				break;
			case (int) IO.SerializationMode.FUNCTION_PALETTE:
				DeserializeJSONToFunctionPalette(configData);
				break;
		}

		// Send out succesful deserialization signal.
		EmitSignal(SignalName.ComponentDeSerialized, path);
	}

	/// <summary>
	/// Serializes the applications current state by turning all relevant infomration to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds all necessary information to load the application from said file.</returns>
	private string SerializeCompleteDeckToJSON() {

		var res = new Dictionary();

		// Serialize FunctionPalette.
		if(functionPalette != null) 
		{
			functionPalette.Name = componentIdentifier + ".FunctionPalette";
			res.Add("FunctionPalette", functionPalette.Save());
		}

		// Serialize UpperTimeline.
		if(upperTimeline != null) 
		{
			upperTimeline.Name = componentIdentifier + ".UpperTimeline";
			res.Add("UpperTimeline", upperTimeline.Save());
		}

		// Serialize LowerTimeline.
		if(lowerTimeline != null) 
		{
			lowerTimeline.Name = componentIdentifier + ".LowerTimeline";
			res.Add("LowerTimeline", lowerTimeline.Save());
		}

		return Json.Stringify(res, "\t");
	}

	/// <summary>
	/// Serializes the Upper and Lower Timelines to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds the timelines.</returns>
	private string SerializeTimelineToJSON() 
	{
		var res = new Dictionary();

		// Serialize UpperTimeline.
		if(upperTimeline != null) 
		{
			upperTimeline.Name = componentIdentifier + ".UpperTimeline";
			res.Add("UpperTimeline", upperTimeline.Save());
		}

		// Serialize LowerTimeline.
		if(lowerTimeline != null) 
		{
			lowerTimeline.Name = componentIdentifier + ".LowerTimeline";
			res.Add("LowerTimeline", lowerTimeline.Save());
		}

		return Json.Stringify(res, "\t");
	}

	/// <summary>
	/// Serializes the Function Palette to JSON.
	/// </summary>
	/// <returns>Returns the JSON file that holds the function palette.</returns>
	private string SerializeFunctionPaletteToJSON() 
	{
		// Serialize FunctionPalette.
		var res = new Dictionary();
		if(functionPalette != null) 
		{
			functionPalette.Name = componentIdentifier + ".FunctionPalette";
			res.Add("FunctionPalette", functionPalette.Save());
		}	
		return Json.Stringify(res, "\t");
	}

	/// <summary>
	/// Deserializes a JSON file which holds the information to load a previous complete save of the application.
	/// </summary>
	/// <param name="configData">Dictionary that holds configuration data for Function Palette and Timeline that make up this deck.</param>
	private void DeserializeJSONToCompleteDeck(Dictionary configData) 
	{
		// Grab component configuration data.
		var paletteConfig = configData["FunctionPalette"].AsGodotDictionary();    
		var upperTimelineConfig = configData["UpperTimeline"].AsGodotDictionary();        
		var timelineConfig = configData["LowerTimeline"].AsGodotDictionary();  		

		// Load all components.
		LoadFunctionPalette(paletteConfig);
		LoadUpperTimeline(upperTimelineConfig);
		LoadLowerTimeline(timelineConfig);
	}  

	/// <summary>
	/// Deserializes a JSON file which holds the information to load a previously saved Function Palette of the application.
	/// </summary>
	/// <param name="configData">Dictionary that holds configuration data for the Function Palette.</param>
	private void DeserializeJSONToFunctionPalette(Dictionary configData) 
	{
		// Grab and load function palette configuration data.
		var paletteConfig = configData["FunctionPalette"].AsGodotDictionary();  
		LoadFunctionPalette(paletteConfig);  
	}

	/// <summary>
	/// Deserializes a JSON file which holds the information to load a previously saved Timeline of the application.
	/// </summary>
	/// <param name="configData">Dictionary that holds configuration data for the Timeline.</param>
	private void DeserializeJSONToTimeline(Dictionary configData) 
	{
		// Grab and load upper+lower timeline configuration data.
		var timelineConfig = configData["LowerTimeline"].AsGodotDictionary(); 
		LoadTimeline(timelineConfig);	
	}

	/// <summary>
	/// Loads a FunctionPalette instance into the application.
	/// </summary>
	/// <param name="functionPaletteDictionary">Dictionary holding information needed to create a LowerTimeline.</param>
	private void LoadFunctionPalette(Dictionary functionPaletteDictionary) 
	{
		// Logic to load the function palette from the file as a new function palette.

		EmitSignal(SignalName.FunctionPaletteLoaded);
	}

	private void LoadTimeline(Dictionary timelineDictionary) {
		LoadLowerTimeline(timelineDictionary);	
	}	

	/// <summary>
	/// Loads an UpperTimeline instance into the application.
	/// </summary>
	/// <param name="timelineDictionary">Dictionary holding information needed to create a LowerTimeline.</param>
	private void LoadUpperTimeline(Dictionary timelineDictionary) 
	{

		// Send out function list to uppertimeline.
		EmitSignal(SignalName.UpperTimelineLoaded);
	}

	/// <summary>
	/// Loads a LowerTimeline instance into the application.
	/// </summary>
	/// <param name="timelineDictionary">Dictionary holding information needed to create a LowerTimeline.</param>
	private void LoadLowerTimeline(Dictionary timelineDictionary) 
	{
		// Grab all LowerTimeline information.
		var timelineName = timelineDictionary["Name"].AsString();
		var timelineCount = timelineDictionary["Count"].AsInt32();
		var timelineFunctionDict = timelineDictionary["Functions"].AsGodotDictionary();
	
		// Create info.
		List<Function> newFunctionList = new();
		TimelineFunctionContainer container = upperTimeline!.timelineFunctionContainer!.Instantiate<TimelineFunctionContainer>();
		HBoxContainer timelineContainer = new HBoxContainer();

		// Iterate through functions and add to a new list.
		for(int i = 0; i < timelineCount; i++) {
			// Get Function Information.
			var currFuncDict = timelineFunctionDict[i.ToString()].AsGodotDictionary();
			var endTime = (int) currFuncDict["EndTime"];
			var startTime = (int) currFuncDict["StartTime"];
			var textRepresentation = (string) currFuncDict["TextRepresentation"];

			// Create Function and add it to the list.
			Function newFunction = new(textRepresentation, Bridge.Parse(textRepresentation).Unwrap(), startTime, endTime);

			newFunctionList.Add(newFunction);

			container.StartTime = newFunction.StartTime;
			container.EndTime = newFunction.EndTime;
			container.LatexString = newFunction!.FunctionAST!.Latex;
			container.Timeline = lowerTimeline;
			container.Index = lowerTimeline!.Count - 1;
			timelineContainer.AddChild(container);
		}

		// Update Timeline.
		lowerTimeline!.Reset();
		lowerTimeline.Name = timelineName;
		lowerTimeline.SetProcess(false);
		lowerTimeline.Add(newFunctionList);

		// Update lower timeline.
		upperTimeline!.Name = timelineName;
		var childCount = upperTimeline.timelineContainer!.GetChildCount();
		for(int i = childCount; i > 0; i++) {
			Node n = upperTimeline.timelineContainer.GetChild(i);
			n.QueueFree();
			upperTimeline.timelineContainer.RemoveChild(n);
		}
		upperTimeline.timelineContainer = timelineContainer;

		// Send out the new lower timeline.
		EmitSignal(SignalName.LowerTimelineLoaded);
		EmitSignal(SignalName.UpperTimelineLoaded);
	}

}
