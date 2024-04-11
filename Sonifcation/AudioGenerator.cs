using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;
using System.Diagnostics;
using Newtonsoft.Json;

using Functions;
namespace Sonification;

/// <summary>
/// Main Audio Generation Node. 
/// </summary>
public partial class AudioGenerator : Node {

	/// <summary>
	/// LowerTimeline representation of the Timeline UI Node
	/// </summary>
	public LowerTimeline timeline = new LowerTimeline();

	/// <summary>
	/// If <c> true </c>, AudioGenerator playback is active.
	/// </summary>
	public bool IsPlaying {get;set;}

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {

		// Add and prep the Timeline.
		AddChild(timeline);
		timeline.SetProcess(false);
		
		// Connect the necessary signals from the Timeline.
		timeline.AudioPlaybackFinished += OnAudioPlaybackFinished;

		// Add 'Tests' that demonstrate the audio playback.
		AddTests();
	}

	/// <summary>
	/// Handles the case where the timeline has indicated audio playback is finished.
	/// </summary>
	private void OnAudioPlaybackFinished() {
		IsPlaying = false;
		GD.Print("---Playback Over---");
	}

	/// <summary>
	/// Begins AudioGenerator audio playback from the beginning of the Timeline.
	/// </summary>
	public void Play() {
		AudioDebugging.Output("AudioGenerator.Play():");
		if(!IsPlaying) {
			timeline.StartPlaying();
			IsPlaying = true;
		}
	}

	/// <summary>
	/// Assorted test Functions organized and added to the timeline for playback.
	/// </summary>
	public void AddTests() {
		IFunctionAST t = new Variable("t");
		IFunctionAST two = new Number(2);
	 	IFunctionAST three = new Number(3);
		IFunctionAST four = new Number(4);
		IFunctionAST five = new Number(5);
		IFunctionAST pi = new Number(3.14159);
		IFunctionAST t_squared = new Exponent(t, two);
		IFunctionAST three_t = new Multiply(three, t);
		IFunctionAST partial = new Add(t_squared,three_t);
		IFunctionAST poly = new Add(partial, four);
		IFunctionAST three_t_plus_four = new Add(three_t, four);
		IFunctionAST sin_3t = new Sine(three_t);
		IFunctionAST three_pi_t = new Multiply(three_t, pi);
		IFunctionAST sin_3_pi_t = new Sine(three_pi_t);
		IFunctionAST inverse = new Divide(two,t);
		IFunctionAST tan = new Tangent(three_pi_t);
		IFunctionAST A4 = new Number(49);
		IFunctionAST zero = new Number(0);
		IFunctionAST negA4 = new Number(-49);
		IFunctionAST sin_t = new Sine(t);
		IFunctionAST three_sin_t = new Multiply(three, sin_t);
		IFunctionAST sin3t_plus_3sint = new Add(sin_3t, three_sin_t);
		IFunctionAST t_plus_5 = new Add(t, five);

		Function tPlus5Func = new Function("t + 5", t_plus_5);
		
		Function sumOfSines = new Function("sin(3t) + 3sin(t)", sin3t_plus_3sint);
		Function negA4Func = new Function("-49", negA4);
		Function zeroFunc = new Function("0", zero);
		Function A4Func = new Function("49", A4);
		Function tanFunc = new Function("Tan(3*pi*t)", tan);
		Function inverseFunc = new Function("2/t)", inverse);
		Function sineFunc2 = new Function("Sin(3*t)", sin_3t);
		Function sineFunc1 = new Function("sin(3*pi*t)", sin_3_pi_t);
		Function linearFunc = new Function("3t + 4", three_t_plus_four);
		Function squareFunc = new Function("t^2", t_squared);
		Function polynomialFunc = new Function("t^2 + 3t + 4", poly);
		/**/

		timeline.Add(tPlus5Func);
		
		timeline.Add(sumOfSines);
		timeline.Add(inverseFunc);
		timeline.Add(polynomialFunc);
		timeline.Add(squareFunc);
		timeline.Add(linearFunc);
		timeline.Add(sineFunc1);
		timeline.Add(sineFunc2);
		timeline.Add(tanFunc);
		timeline.Add(A4Func);
		timeline.Add(zeroFunc);
		timeline.Add(negA4Func);
		/**/

	}

	public void Serialize(){
		GD.Print("Serializing...");
		var timelineSave = timeline.Save();
		var str = JsonConvert.SerializeObject(timeline);
		string path = ProjectSettings.GlobalizePath("user://timeline.json");

		// Serialize object directly into File Stream.
        using StreamWriter file = File.CreateText(path);
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(file, timeline);

    }

	public void Deserialize() {
		GD.Print("Deserializing...");
		string path = ProjectSettings.GlobalizePath("user://timeline.json");

		// Serialize object directly into File Stream.
        string jsonString = File.ReadAllText(path);
		GD.Print(jsonString);
        //LowerTimeline? timeline2 = JsonConvert.DeserializeObject<LowerTimeline>(jsonString);

		// Deserialize JSON directly from a file
		using (StreamReader file = File.OpenText(path))
		{
			JsonSerializer serializer = new JsonSerializer();
            LowerTimeline timeline2 = (LowerTimeline)serializer.Deserialize(file, typeof(LowerTimeline));
		}
	
	}
	
}
