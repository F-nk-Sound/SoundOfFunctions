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

/// <summary>
/// Provides lookup table of note to frequency mappings. <br/>
/// Access Example : Frequencies.Notes[n] returns the frequency to which note n points to. <br/>
/// 'n' corresponds with the same 'n' in this table from wikipedia: <br/>
/// https://en.wikipedia.org/wiki/Piano_key_frequencies#List
/// </summary>
public static class Frequencies {

	/// <summary>
	/// Key number (out of 88) tuning is based on. Default = 49 = A4.
	/// </summary>
	private static int tuningKeyNum = 49;						

	/// <summary>
	/// Tuning frequency (in Hz) corresponding tuning key. Default = A4 = 440 Hz.
	/// </summary>
	private static double tuningFreq = 440;	

	/// <summary>
	/// Number of possible notes.
	/// </summary>
	public static int resolution = 88;	

	/// <summary>
	/// Lookup table of note to frequency mappings basedon an 88-key piano.
	/// </summary>
	public static Dictionary<int, double> Notes = new Dictionary<int, double>(resolution);		

	static Frequencies() {
		FillDictionary();	
	}

	/// <summary>
	/// Fills the note to frequency lookup table via formula found from wikipedia.
	/// </summary>
	static void FillDictionary() {
		for(int n = 1; n <= resolution; n++) {
			double freq = Math.Pow(2, (n - tuningKeyNum)/12) * tuningFreq;
			Notes.Add(n, freq);
		}
	}
	
}

public partial class Function : Node {
	private string textRepresentation;		// Input text funtion is parsed from
	private string type;					// Type of Function: i.e., Cubic, Logarithimic, Linear
	private int runLength;					// Size of domain	
	private int startPos;					// Starting time point
	private int endPos;						// Ending time point
	private List<int> timeDomain;			// Function domain: [startPos, ..., endPos]
	private List<int> noteSequence;			// Frequency domain: [minFreq, ..., maxFreq]
	private AudioStreamPlayer player;		// Audio Player
	public bool isPlaying;			// Duh

	public Function(string textRepresentation, int startPos, int endPos) {
		this.textRepresentation = textRepresentation;
		timeDomain = fillTimeDomain(textRepresentation, startPos, endPos);
		noteSequence = fillNoteSequence(timeDomain);
		this.startPos = startPos;
		this.endPos = endPos;
		runLength = endPos - startPos;
		isPlaying = false;

		// Audio stream characteristics
		player = new AudioStreamPlayer();
		player.Stream = new AudioStreamGenerator {
			BufferLength = runLength,
			MixRate = 44100
		};

		AddChild(player);
	}

	private List<int> fillTimeDomain(string textRepresentation, int startPos, int endPos) {
		List<int> res = new List<int>();
		/*
		Do the stuff to get the values
		*/
		return res;
	}
	private List<int> fillNoteSequence(List<int> timeDomain) {
		List<int> res = new List<int>();
		return res;
	}

	private Vector2[] generateAudio(int noteNum) {
		
		// Calculate buffer length and then adjust it to seconds
		var duration = timeDomain.Last<int>() - timeDomain.First<int>();
		var adjDuration = duration * duration/8;
		((AudioStreamGenerator) player.Stream).BufferLength = (float) adjDuration;
		
		// Calculate the buffer size and create  the array to store audio data
		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		int bufferSize = (int) (sampleRate * adjDuration);
		Vector2[] audio = new Vector2[bufferSize];

		// Fill the buffer appropriately 
		var phase = 0.0;
		double increment = Frequencies.Notes[noteNum] / sampleRate;
		for (int i = 0; i < bufferSize; i++) {
			var sample = Vector2.One * (float) (4 * Mathf.Sin(Mathf.Tau * phase));
			audio[i] = sample;
			phase += increment;
		}	
	
		return audio;
	}

	private Vector2[] generateNoteAudio(int noteNum) {
		
		// Calculate buffer length and then adjust it to seconds
		var duration = 0.75;
		((AudioStreamGenerator) player.Stream).BufferLength = (float) duration;
		
		// Calculate the buffer size and create  the array to store audio data
		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		int bufferSize = (int) (sampleRate * duration);
		Vector2[] audio = new Vector2[bufferSize];

		// Fill the buffer appropriately 
		var phase = 0.0;
		double increment = Frequencies.Notes[noteNum] / sampleRate;
		for (int i = 0; i < bufferSize; i++) {
			var sample = Vector2.One * (float) (4 * Mathf.Sin(Mathf.Tau * phase));
			audio[i] = sample;
			phase += increment;
		}	
	
		return audio;
	}
	
	public void playFunction() { 
		// Generate the audio data for each note
		if (!noteSequence.Any()) addDefaultNotes();
		player.Play();
		var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
		
		int noteIndex = 0;
		while(noteIndex != noteSequence.Count) {
			var sample = generateNoteAudio(noteSequence[noteIndex]);
			playback.PushBuffer(sample);
			//if(player.Stream)
		}
	}

	public void stopFunction() {
		isPlaying = false;
	}
	private void addDefaultNotes() {
		for(int i = 32; i < 44; i+=1) noteSequence.Add(i);
	}
	public int length() {
		return runLength;
	}

}

public partial class Timeline : Node {
	private int length;					// Length of timeline
	private AudioStreamPlayer player;	// Stream to play timeline
	private List<Function> functions;	// List of Functions on timeline

	private bool isPlaying;				// Timeline active playback or not

	public Timeline() {
		length = 0;
		functions = new List<Function>();
		
		// Audio characteristics 
		player = new AudioStreamPlayer();
		player.Stream = new AudioStreamGenerator();

		GetNode($".").AddChild(player);
	}

	public void AddFunction(Function func) {
		//GetNode($".").AddChild(func);		// NEW, See if adding this here and removing the addition of  functions as kids to root still works
		length += func.length();
		functions.Add(func);
	}

	public Boolean RemoveFunction(Function func) {
		var removed = functions.Remove(func);
		if(removed) length -= func.length();
		return removed; 
	}

	public void Play() {
		// Play the stream of each function 
		foreach (var func in functions) {
			func.playFunction();
			Task.Delay(1000 * func.length());
		}
	}

	public void Play(AudioStreamPlayer player) {
		// Play the timeline as a single stream
		
		foreach (Function func in functions) {
			// Add the function note list into the stream of the timeline
		}
	}

	public void Save() {

	}

	public void Delete() {
		
	}
}

public partial class PlayFunctions : Node {

	private List<AudioStreamPlayer> strmPlayerList = new List<AudioStreamPlayer>();
	private List<AudioStreamGenerator> strmGenList = new List<AudioStreamGenerator>();

	static void PlayNoteSimple(AudioStreamPlayer player, int n) {

		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		player.Play();
		AudioStreamGeneratorPlayback playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
		
		double phase = 0.0;
		int frames = playback.GetFramesAvailable();
		double increment = Frequencies.Notes[n] / sampleRate;
		for (int i = 0; i < frames; i++) {
			playback.PushFrame(Vector2.One * (float)(4 * Mathf.Sin(phase * Mathf.Tau)));
			phase += increment;
		}	
	}

	public Vector2[] generateTone(AudioStreamPlayer player, double dur, int noteNum) {
		
		float sampleRate = ((AudioStreamGenerator) player.Stream).MixRate;
		((AudioStreamGenerator) player.Stream).BufferLength = (float) (dur * dur/8);
		int bufferSize = (int) (sampleRate * dur * dur/8);
		Vector2[] audio = new Vector2[bufferSize];
		double increment = Frequencies.Notes[noteNum] / sampleRate;
		var phase = 0.0;
		for (int i = 0; i < bufferSize; i++) {
			var sample = Vector2.One * (float) (4 * Mathf.Sin(Mathf.Tau * phase));
			audio[i] = sample;
			phase += increment;
		}	
	
		return audio;
	}

	public void playTone(AudioStreamPlayer player, Vector2[] audio) {
		player.Play();
		var playback = (AudioStreamGeneratorPlayback) player.GetStreamPlayback();
		playback.PushBuffer(audio);
	}

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {
		
		Function linear = new Function("y = 2x + 6", 1, 5);
		Function cubic = new Function("y = x^3 + 2", 5, 10);
		
		// Add the functions to the scene tree
		linear.Name = "linear";
		cubic.Name = "cubic";
		GetNode($".").AddChild(linear);
		GetNode($".").AddChild(cubic);
	
	}

	// Process sentinels
	bool played = false;
	double duration = -1;
	double time = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
		// Actually play functions
		var child0 = GetChild(0);
		if (child0 is Function func && !played) {
			if(!func.isPlaying) {
				func.playFunction();
				duration = (0.75 * 12) + 5.0;
				played = true;
			}
			else {
				time += delta;
				if(time >= duration) {
					func.stopFunction();
					duration = -1;
					time = 0;
				}
			}
		}


	}
}
