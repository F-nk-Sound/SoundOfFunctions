using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Functions;
using System.Runtime.CompilerServices;
using System.Data.Common;
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
	/// If <c> true </c>, Function is a constant function (i.e., y = 1, y = 3, y = 6942).
	/// </summary>
	public bool IsConstant {get; set;}

	/// <summary>
	/// Function starting time point. <br/>
	/// </summary>
	public int StartTime {
		get {
			return _startTime;
		}
		set {
			// If the start time has changed, reset the function table and note sequence.
			if(value != _startTime) {
				_startTime = value;
				Refresh();
			}
		}
	}	
	private int _startTime;	

	/// <summary>
	/// Function ending time point. <br/>
	/// </summary>
	public int EndTime {
		get {
			return _endTime;
		}
		set {
			// If the end time has changed, reset the function table and note sequence.
			if(value != _endTime) {
				_endTime = value;
				Refresh();
			}
		}
	}	
	private int _endTime;		

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
		_startTime = -5;
		_endTime = 5;
		
		// Characteristics relevant to audio playback
		CurrNote = 0;
		NoteDuration = 0.125;
		RunTime = _endTime - _startTime;
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

		// Iterate over the Domain and fill in the Function range, taking special care to tag Function as constant if needed.
		IsConstant = true;
		double lastValue = FunctionAST.EvaluateAtT(StartTime);
		FunctionTable.Add(lastValue);

		for(double t = StartTime + NoteDuration; t <= EndTime; t += NoteDuration) {

			// Evalauate the funtion at the approriate time.
			var value = FunctionAST.EvaluateAtT(t);
			
			// Manage constant/nonconstant function case.
			if(IsConstant) {
				if(value != lastValue) IsConstant = false;
				else lastValue = value;
			}

			// Manage out of bounds case.
			if(Math.Abs(value) == double.PositiveInfinity) value = 0;

			// Add the value to the functions table.
			FunctionTable.Add(value);
		}

	}
	
	/// <summary>
	/// Uses Function <c>FunctionTable</c> to calculate the appropriate values for the functions audio sequence.
	/// </summary>
	private void FillNoteSequence() {

		// Functions have a set amount of notes to choose from defined impliclty in 'Frequencies.cs'.
		int noteNumStart = Frequencies.startingNoteNumber;
		int noteNumEnd = Frequencies.endingNoteNumber;
		int noteRange = noteNumEnd - noteNumStart;
		
		// Find the full range of values that the function can take.
		double minVal = FunctionTable.Min();
		double maxVal = FunctionTable.Max();
		double functionRange = maxVal - minVal;
		
		// Map the Functions values to note numbers
		foreach(double value in FunctionTable) {
			int noteNumber;
			if(IsConstant) noteNumber = (int) Math.Abs(FunctionTable[0]) % noteRange;
			else {
				double normalizedValue = (value - minVal) / functionRange;
				noteNumber = noteNumStart + (int) (normalizedValue * noteRange);
			}
			noteSequence.Add(noteNumber);
		}

	}

	/// <summary>
	/// Recomputes Function Table and Note Sequence.
	/// </summary>
	private void Refresh() {
		RunTime = _endTime - _startTime;
		FillFunctionTable();
		FillNoteSequence();
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
			var sample = Vector2.One * (float) (1 * Mathf.Sin(Mathf.Tau * phase));
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
		if(currTime != RunTime) return false;
		
		// Stopping Sonification
		CurrNote = 0;
		player.Stop();
		SetProcess(false);
		timer.Reset();
		AudioDebugging.Output("Function Stopped");
		return true;
		
	}

	/// <summary>
	/// Begins audio function playback.
	/// </summary>
	public bool StartPlaying() {
		
		AudioDebugging.Output("Entered Function.StartPlaying()");
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

		if(StopPlaying()) return;

		// Push new note into Audio Buffer on each discrete timer tick
		bool playNextNote = ((int) timer.ElapsedTime % NoteDuration) == 0;
		if(CurrNote != noteSequence.Count && playNextNote) {
			var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
			var sample = GenerateNoteAudio(noteSequence[CurrNote]);
			playback.PushBuffer(sample);
			CurrNote++;
		}
		
		if(timer.IsTimeChanged) AudioDebugging.Output("\t->F:(" + Name + ").Timer.CurrTime = " + timer.ClockTimeRounded + " s.");
	}

	public override void _Process(double delta) {
		timer.Tick(delta);
		Play();
	}

}

