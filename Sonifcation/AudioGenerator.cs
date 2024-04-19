using Godot;
using Functions;
using Parsing;

namespace Sonification;

/// <summary>
/// Main Audio Generation Node. 
/// </summary>
public partial class AudioGenerator : Node {

	/// <summary>
	/// LowerTimeline representation of the Timeline UI Node
	/// </summary>
	[Export]
	public LowerTimeline? timeline;

	/// <summary>
	/// If <c> true </c>, AudioGenerator playback is active.
	/// </summary>
	public bool IsPlaying { get;set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Add and prep the Timeline.
		timeline!.SetProcess(false);

		// Add 'Tests' that demonstrate the audio playback.
		AddTests();
		if(AudioDebugging.Enabled) 
		{
			AudioDebugging.Output("Displaying AudioGenerator SceneTree after adding intial LowerTimeline");
			PrintTreePretty();
		}
	}

	private void OnLowerTimelineUpdated() {
		// Introduce the new timeline.
		AudioDebugging.Output("\tNew Timeline Processing Before Added as Child: " + timeline!.IsProcessing());
		foreach (Node n in timeline.GetChildren()) 
		{
			if (n is Function) n.SetProcess(false);
		}
		AudioDebugging.Output("\tNew Timeline Processing After Added as Child: " + timeline.IsProcessing());
		AudioDebugging.Output("Added the New LowerTimeline");

		if (AudioDebugging.Enabled) 
		{
			AudioDebugging.Output("Displaying Current AudioGenerator SceneTree after updating LowerTimeline.");
			PrintTreePretty();
			AudioDebugging.Output("Examining the processing of each child node");
			foreach (Node n in GetChildren()) 
			{
				AudioDebugging.Output("\tNode: " + n.Name + " Processing? " + n.IsProcessing());
				var grandkids = n.GetChildren();
				if (grandkids.Count != 0) 
				{
					AudioDebugging.Output("\tExamining the processing of each function node");
					foreach (Node gk in grandkids) 
					{
						if (gk is Function) AudioDebugging.Output("\t\tFunction: " + gk.Name + " Processing? " + gk.IsProcessing());
					}
				}
			}
		}
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
		AudioDebugging.Output("AudioGenerator.Play(): IsPlaying = " + IsPlaying);
		if(!IsPlaying) {
			if (timeline!.StartPlaying())
				IsPlaying = true;
		}
	}

	/// <summary>
	/// Assorted test Functions organized and added to the timeline for playback.
	/// </summary>
	public void AddTests() {
		// IFunctionAST t_squared = Bridge.Parse("t^2").Unwrap();
		// IFunctionAST poly = Bridge.Parse("t^2 + 3t + 4").Unwrap();
		// IFunctionAST three_t_plus_four = Bridge.Parse("3t + 4").Unwrap();
		// IFunctionAST sin_3t = Bridge.Parse("sin(3*t)").Unwrap();
		// IFunctionAST sin_3_pi_t = Bridge.Parse("sin(3 * 3.14159 * t)").Unwrap();
		// IFunctionAST inverse = Bridge.Parse("2/t").Unwrap();
		// IFunctionAST tan = Bridge.Parse("tan(3t)").Unwrap();
		// IFunctionAST A4 = Bridge.Parse("49").Unwrap();
		// IFunctionAST zero = Bridge.Parse("0").Unwrap();
		// IFunctionAST negA4 = Bridge.Parse("-49").Unwrap();
		// IFunctionAST sin3t_plus_3sint = Bridge.Parse("sin(3t) + 3sin(t)").Unwrap();
		// IFunctionAST t_plus_5 = Bridge.Parse("t + 5").Unwrap();

		// // Function inverseFunc = new Function("2/t", inverse);		
		// // Function sumOfSines = new Function("sin(3t) + 3sin(t)", sin3t_plus_3sint);
		// // Function negA4Func = new Function("-49", negA4);
		// // Function zeroFunc = new Function("0", zero);

		// // Function tanFunc = new Function("tan(3t)", tan);
		// // Function tPlus5Func = new Function("t + 5", t_plus_5);
		// string func = "1000t";
		// Function f = new(func, Bridge.Parse(func).Unwrap())
		// {
		// 	EndTime = 10,
		// };
		// Function sineFunc1 = new Function("sin(3*3.14159*t)", sin_3_pi_t);
		// Function linearFunc = new Function("3t + 4", three_t_plus_four);
		// Function squareFunc = new Function("t^2", t_squared);
		// Function polynomialFunc = new Function("t^2 + 3t + 4", poly);
		
		// Function A4Func = new Function("49", A4);
		// timeline.Add(A4Func);

		/*
		timeline.Add(inverseFunc);
		timeline.Add(tPlus5Func);
		timeline.Add(sumOfSines);
		timeline.Add(polynomialFunc);
		timeline.Add(squareFunc);
		timeline.Add(linearFunc);
		timeline.Add(sineFunc1);
		timeline.Add(sineFunc2);
		timeline.Add(tanFunc);
		
		timeline.Add(zeroFunc);
		timeline.Add(negA4Func);
		//*/

	}
	
}
