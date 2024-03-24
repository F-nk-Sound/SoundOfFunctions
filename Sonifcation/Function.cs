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
	private string TextRepresentation;		

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
	/// <param name="name">This Functions Name (i.e., its text representation before parsing).</param>
	/// <param name="functionAST">The AST that represents the function.</param>
	public Function(string name, IFunctionAST functionAST) {
		// Banal Characteristics
		Name = name;
		FunctionAST = functionAST;

		// Default start and stop
		StartTime = -5;
		EndTime = 5;

		// Initialize playback and Graph representation materials
		FunctionTable = new List<double>();
		noteSequence = new List<int>();
		FillFunctionTable();
		FillNoteSequence();
		
		// Characteristics relevant to audio playback
		CurrNote = 0;
		NoteDuration = 1.0;
		RunTime = EndTime - StartTime;
		timer = new TimeKeeper();
		player = new AudioStreamPlayer {
			Stream = new AudioStreamGenerator {
				BufferLength = RunTime,
				MixRate = 44100,
			},
			Name = name + "_Player"
		};

		// Add player to scene tree
		SetProcess(false);
		AddChild(player);
	}

	/// <summary>
	/// Uses Function text representation to calculate the appropriate values of the normal time domain of the function.
	/// </summary>
	private void FillFunctionTable() {
		if(FunctionAST == null) return;
		// Iterate over the Domain and fill in the function's range
		for(int t = StartTime; t <= EndTime; t++) {
			var value = FunctionAST.EvaluateAtT(t);
			FunctionTable.Add(value);
		}
	}
	
	/// <summary>
	/// Uses Function <c>FunctionTable</c> to calculate the appropriate values for the functions audio sequence.
	/// </summary>
	private void FillNoteSequence() {

		// Should be removed once demos concluded
		if(Name == "Lucid Dreams") addLucidDreams();
		if(Name == "Twinkle Twinkle") addTwinkleTwinkle();
		if(Name == "Seven Nation Army") addSevenNationArmy();
		if(Name == "Scale") addDefaultNotes();
		if(Name == "Hot Cross Buns") addHotCrossBuns();
		if(FunctionTable.Count == 0) return;
		
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
		Play();

		// Successful initialization
		return true;
	}	

	/********* Below Only for Testing and Demonstration Purposes *********/
	private void addDefaultNotes() {
		for(int i = 32; i < 32 + 12; i += 1) noteSequence.Add(i);
		StartTime = 23;
		EndTime = 23 + 12;
		RunTime = 12;
	}

	private void addHotCrossBuns() {
		int E4 = 44;
		int D4 = 42;
		int C4 = 40;
		
		noteSequence.Add(E4);
		noteSequence.Add(D4);
		noteSequence.Add(C4);

		noteSequence.Add(E4);
		noteSequence.Add(D4);
		noteSequence.Add(C4);
		
		noteSequence.Add(C4);
		noteSequence.Add(C4);
		noteSequence.Add(C4);
		noteSequence.Add(C4);

		noteSequence.Add(D4);
		noteSequence.Add(D4);
		noteSequence.Add(D4);
		noteSequence.Add(D4);

		noteSequence.Add(E4);
		noteSequence.Add(D4);
		noteSequence.Add(C4);

		StartTime = 5;
		EndTime = 5 + 17;
		RunTime = 17;

	}

	private void addSevenNationArmy() {
		int C3 = 28;
		int Eb3 = 31;
		int Bb3 = 38;
		int Ab3 = 36;
		int G3 = 35;

		for(int i = 0; i < 3; i++) {
			noteSequence.Add(C3);
			noteSequence.Add(C3);
			noteSequence.Add(Eb3);
			noteSequence.Add(C3);
			noteSequence.Add(Bb3);
			noteSequence.Add(Ab3);
			noteSequence.Add(G3);
		}

		StartTime = 35;
		EndTime = 35 + 21;
		RunTime = 21;
	}

	private void addTwinkleTwinkle() {
		int A4 = 49;
		int G4 = 47;
		int E4 = 44;
		int D4 = 42;
		int C4 = 40;
		int F4 = 49;

		noteSequence.Add(C4);
		noteSequence.Add(C4);
		noteSequence.Add(G4);
		noteSequence.Add(G4);

		noteSequence.Add(A4);
		noteSequence.Add(A4);
		noteSequence.Add(G4);
		noteSequence.Add(F4);
		
		noteSequence.Add(F4);
		noteSequence.Add(E4);
		noteSequence.Add(E4);
		noteSequence.Add(D4);

		noteSequence.Add(D4);
		noteSequence.Add(C4);
		noteSequence.Add(G4);
		noteSequence.Add(G4);

		noteSequence.Add(F4);
		noteSequence.Add(F4);
		noteSequence.Add(E4);
		noteSequence.Add(E4);

		noteSequence.Add(D4);
		noteSequence.Add(G4);
		noteSequence.Add(G4);
		noteSequence.Add(F4);
		
		noteSequence.Add(F4);
		noteSequence.Add(E4);
		noteSequence.Add(E4);
		noteSequence.Add(C4);

		noteSequence.Add(C4);
		noteSequence.Add(G4);
		noteSequence.Add(G4);
		noteSequence.Add(A4);

		noteSequence.Add(A4);
		noteSequence.Add(G4);
		noteSequence.Add(F4);
		noteSequence.Add(F4);

		noteSequence.Add(E4);
		noteSequence.Add(E4);
		noteSequence.Add(D4);
		noteSequence.Add(D4);

		noteSequence.Add(C4);

		StartTime = 57;
		EndTime = 57 + 41;
		RunTime = 41;
	}

	private void addLucidDreams() {
		int A4 = 49;
		int Db4 = 41;
		int Ab4 = 48;
		int Fs4 = 46;
		int B4 = 51;
		int F4 = 45;
		int D4 = 42;
		int E4 = 44;

		noteSequence.Add(A4);
		noteSequence.Add(Db4);
		noteSequence.Add(A4);
		noteSequence.Add(Ab4);
		noteSequence.Add(Db4);
		noteSequence.Add(Fs4);
		noteSequence.Add(B4);
		noteSequence.Add(Fs4);
		noteSequence.Add(Fs4);
		noteSequence.Add(F4);

		noteSequence.Add(A4);
		noteSequence.Add(D4);
		noteSequence.Add(A4);
		noteSequence.Add(Ab4);
		noteSequence.Add(Db4);
		noteSequence.Add(Fs4);
		noteSequence.Add(B4);
		noteSequence.Add(Fs4);
		noteSequence.Add(Fs4);
		noteSequence.Add(F4);
		noteSequence.Add(F4);

		noteSequence.Add(D4);
		noteSequence.Add(Fs4);
		noteSequence.Add(D4);
		noteSequence.Add(A4);
		noteSequence.Add(Fs4);
		noteSequence.Add(D4);
		noteSequence.Add(E4);
		noteSequence.Add(A4);
		noteSequence.Add(Db4);
		noteSequence.Add(Ab4);
		noteSequence.Add(F4);
		noteSequence.Add(Fs4);
		noteSequence.Add(E4);
		noteSequence.Add(D4);
		noteSequence.Add(Db4);
		noteSequence.Add(B4);
		noteSequence.Add(Db4);
		noteSequence.Add(Db4);
		noteSequence.Add(Db4);

		StartTime = 98;
		EndTime = 98 + 40;
		RunTime = 40;

	}
	/********* Above Only for Testing and Demonstration Purposes *********/

	/// <summary>
	/// Pushes the current note from noteSequence[] into the buffer.
	/// </summary>
	private void Play() { 
		// Stop playback if necessary
		if(StopPlaying()) return;

		// Push new note into Audio Buffer on each discrete timer tick
		bool changed = timer.IsTimeChanged;
		if(CurrNote != noteSequence.Count && changed) {
			var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
			var sample = GenerateNoteAudio(noteSequence[CurrNote]);
			playback.PushBuffer(sample);
			CurrNote++;
		}
	}

	public override void _Process(double delta) {
		timer.Tick(delta);
		Play();
	}

}

