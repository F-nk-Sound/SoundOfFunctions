using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Functions;
namespace Sonification;

/// <summary>
/// Represents any individual function passed into the program as a Node.
/// </summary>
public partial class Function : Node {
	/// <summary>
	/// Input text that function is parsed from. <br/>
	/// </summary>
	private string TextRepresentation {get;}	

	/// <summary>
	/// Stores the AST of the Function.
	/// </summary>
	public IFunctionAST FunctionAST {get; set;}
	/// <summary>
	/// Function starting time point. <br/>
	/// </summary>
	public int StartTime {get; set;}		

	/// <summary>
	/// Function ending time point. <br/>
	/// </summary>
	public int EndTime {get; set;}			

	/// <summary>
	/// Stores the calculated time domain representation of the function with respect to time. <br/>
	/// </summary>
	public List<double> FunctionTable;			

	/// <summary>
	/// Stores the audio sequence of the function as note numbers. <br/>
	/// To access actual frequencies, use <c>Frequencies.Notes[noteSequence[n]]</c>
	/// </summary>
	private List<int> noteSequence;			

	/// <summary>
	/// Function audio source. <br/>
	/// </summary>
	private AudioStreamPlayer player;		
	
	/// <summary>
	/// Function Timer object to manage and synchronize playback. <br/>
	/// </summary>
	private readonly TimeKeeper timer;        

	/// <summary>
	/// Individual note run time (seconds). <br/>
	/// </summary>
	private double NoteDuration {get; set;}  

	/// <summary>
	/// Current note being added to the audio buffer. <br/>
	/// </summary>
	private int CurrNote {get; set;}        

	/// <summary>
	/// Length of time function plays for (seconds). <br/>
	/// </summary>
	public int RunTime {get; set;}			

	/// <summary>
	/// Initializes a new Function Node w/ a domain of [-5,5].
	/// </summary>
	/// <param name="textRepresentation">This Functions Name (i.e., its text representation before parsing).</param>
	/// <param name="functionAST">The AST that represents the function.</param>
	public Function(string textRepresentation, IFunctionAST functionAST) {
		// Banal Characteristics
		Name = textRepresentation;
		TextRepresentation = textRepresentation;
		FunctionAST = functionAST;

		// Default start and stop
		StartTime = -5;
		EndTime = 5;
		
		// Characteristics relevant to audio playback
		CurrNote = 0;
		NoteDuration = 0.125;
		RunTime = EndTime - StartTime;
		FunctionTable = new List<double>();
		noteSequence = new List<int>();
		FillFunctionTable();
		FillNoteSequence();
		timer = new TimeKeeper();
		player = new AudioStreamPlayer {
			Stream = new AudioStreamGenerator {
				BufferLength = RunTime,
				MixRate = 44100,
			},
			Name = "[" + textRepresentation + "]Player"
		};

		// Add player to scene tree
		SetProcess(false);
		AddChild(player);
	}

	/// <summary>
	/// Uses Function text representation to calculate the appropriate values of the normal time domain of the function.
	/// </summary>
	private void FillFunctionTable() {

		// Iterate over the Domain and fill in the function's range
		for(double t = StartTime; t <= EndTime; t += NoteDuration) {
			var value = FunctionAST.EvaluateAtT(t);
			FunctionTable.Add(value);
		}
	}
	
	/// <summary>
	/// Uses Function <c>FunctionTable</c> to calculate the appropriate values for the functions audio sequence.
	/// </summary>
	private void FillNoteSequence() {

		// Functions have 88 notes to choose from.
		int noteNumStart = 1;
		int noteNumEnd = 88;
		int noteRange = noteNumEnd - noteNumStart;
		
		// Find the full range of values that the function can take.
		double minVal = FunctionTable.Min();
		double maxVal = FunctionTable.Max();
		double functionRange = maxVal - minVal;

		// Map the Functions values to note numbers 
		foreach(double value in FunctionTable) {
			double normalizedValue = (value - minVal) / functionRange;
			int note = noteNumStart + (int) (normalizedValue * noteRange);
			noteSequence.Add(note);
		}
	}

	/// <summary>
	/// Generate individual note audio based on functions current playing state.
	/// </summary>
	/// <param name="noteNum">Note pointing to associated frequency</param>
	private Vector2[] GenerateNoteAudio(int noteNum) {
		
		// Grab audio generator
		var streamGenerator = (AudioStreamGenerator) player.Stream;
		
		// Calculate the number of frames to push to the buffer to achieve proper duration
		float sampleRate = streamGenerator.MixRate;
		int bufferSize = (int) (sampleRate * NoteDuration);
		Vector2[] audio = new Vector2[bufferSize];

		// Grabs the actual frequency associated w/ noteNum and calculate shifting
		var phase = 0.0;
		double increment = Frequencies.Notes[noteNum] / sampleRate;

		// Create the frames to fill the buffer with the note for the proper duration
		for (int i = 0; i < bufferSize; i++) {
			var sample = Vector2.One * (float) (4 * Mathf.Sin(Mathf.Tau * phase));
			audio[i] = sample;
			phase += increment;
		}	

		// Return generated note audio
		return audio;
	}

	/// <summary>
	/// Terminates function audio playback if function run time has elapsed.
	/// </summary>
	public bool StopPlaying() {
		// Poll the time and check if time has arrived. Stop sonificaiton if needed
		int currTime = timer.ClockTimeRounded;
		if(currTime != EndTime + RunTime) return false;
		
		// Stopping Sonification
		CurrNote = 0;
		player.Stop();
		SetProcess(false);
		timer.Reset();
		return true;
		
	}

	/// <summary>
	/// Begins audio function playback.
	/// </summary>
	public bool StartPlaying() {

		// Don't allow uninitialized functions to play
		if(noteSequence.Count == 0) throw new Exception("Attempting to Play Empty Function");
		
		// Initializing Sonification
		CurrNote = 0;
		SetProcess(true);
		player.Play();
		timer.BeginTracking();
		Play();

		// Successful initialization
		return true;
	}	

	/// <summary>
	/// Pushes the current note from noteSequence[] into the buffer.
	/// </summary>
	private void Play() { 
	
		// Push new note into Audio Buffer on each discrete timer tick
		bool playNextNote = ((int) timer.ElapsedTime % NoteDuration) == 0;
		if(CurrNote != noteSequence.Count && playNextNote) {
			var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
			var sample = GenerateNoteAudio(noteSequence[CurrNote]);
			playback.PushBuffer(sample);
			CurrNote++;
		}
		
		if(timer.IsTimeChanged && AudioDebugging.Enabled) GD.Print("\t->Function.Timer.CurrTime = " + timer.ClockTimeRounded + " s.");
	}

	public override void _Process(double delta) {
		timer.Tick(delta);
		Play();
	}

}

