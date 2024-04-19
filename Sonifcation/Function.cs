using Godot;

using Functions;
using Newtonsoft.Json;
namespace Sonification;

/// <summary>
/// Represents any individual function passed into the program as a Node.
/// </summary>
public partial class Function : Node
{
	/// <summary>
	/// Input text that function is parsed from. <br/>
	/// </summary>
	[JsonRequired]
	public string TextRepresentation { get; }

	/// <summary>
	/// Stores the AST of the Function.
	/// </summary>
	[JsonIgnore]
	public IFunctionAST FunctionAST { get; set; }

	/// <summary>
	/// Function starting time point. <br/>
	/// </summary>
	public int StartTime { get; set; } = 0;

	/// <summary>
	/// Function ending time point. <br/>
	/// </summary>
	public int EndTime { get; set; } = 1;

	/// <summary>
	/// Function Timer object to manage and synchronize playback. <br/>
	/// </summary>
	private readonly TimeKeeper Timer;

	/// <summary>
	/// Function audio source. <br/>
	/// </summary>
	private AudioStreamPlayer player;

	AudioStreamGenerator Generator => (AudioStreamGenerator)player.Stream;

	AudioStreamGeneratorPlayback Playback => (AudioStreamGeneratorPlayback)player.GetStreamPlayback();

	/// <summary>
	/// Length of time function plays for (seconds). <br/>
	/// </summary>
	public int RunTime => EndTime - StartTime;

	double t;
	double phase;

	/// <summary>
	/// Initializes a new Function Node w/ a domain of [-5,5].
	/// </summary>
	/// <param name="textRepresentation">This Functions Name (i.e., its text representation before parsing).</param>
	/// <param name="functionAST">The AST that represents the function.</param>
	public Function(string textRepresentation, IFunctionAST functionAST)
	{
		// Banal Characteristics
		TextRepresentation = textRepresentation;
		Name = textRepresentation;
		FunctionAST = functionAST;

		// Default start and stop
		t = StartTime;
		Timer = new();

		// Characteristics relevant to audio playback
		player = new AudioStreamPlayer
		{
			Stream = new AudioStreamGenerator
			{
				MixRate = 44100,
			},
			Name = "[" + textRepresentation + "]Player"
		};

		// Add player to scene tree
		AudioDebugging.Output("Function Fully Initialized.");
		AudioDebugging.Output("Function Processing PreSet: " + IsProcessing());
		SetProcess(false);
		AddChild(player);
		AudioDebugging.Output("Function Processing PostSet: " + IsProcessing());
	}

	Vector2 CreateSinWaveFrame(double freq, double sampleRate)
	{
		double increment = freq / sampleRate;
		Vector2 frame = Vector2.One * (float)Mathf.Sin(Mathf.Tau * phase);
		phase = Mathf.PosMod(phase + increment, 1.0);
		return frame;
	}

	/// <summary>
	/// Generate individual note audio based on functions current playing state.
	/// </summary>
	/// <param name="noteNum">Note pointing to associated frequency</param>
	private void GenerateNoteAudio()
	{
		// Calculate the number of frames to push to the buffer to achieve proper duration.
		float sampleRate = Generator.MixRate;

		for (int i = 0; i < Playback.GetFramesAvailable(); i++)
		{
			if (t >= EndTime)
				break;

			double frequency = Frequencies.GetFrequency(FunctionAST.EvaluateAtT(t));

			var frame = CreateSinWaveFrame(frequency, sampleRate);

			Playback.PushFrame(frame);
			t += 1 / sampleRate;
		}
	}

	/// <summary>
	/// Terminates function audio playback if function run time has elapsed.
	/// </summary>
	public bool StopPlaying()
	{
		// Stopping Sonification
		double currFreq = Frequencies.GetFrequency(FunctionAST.EvaluateAtT(t));
		FadeOut(currFreq, 0.05);
		player.Stop();
		SetProcess(false);
		AudioDebugging.Output("Function " + Name + " Stopped");
		return true;
	}

	/// <summary>
	/// Begins audio function playback.
	/// </summary>
	public bool StartPlaying()
	{
		AudioDebugging.Output("Entered Function.StartPlaying()");
		// Initializing Sonification
		SetProcess(true);
		player.Play();

		Timer.Reset();
		Timer.BeginTracking();

		t = StartTime;
		phase = 0.0;

		double initFreq = Frequencies.GetFrequency(FunctionAST.EvaluateAtT(StartTime));
		FadeIn(initFreq, 0.05);
		Play();

		// Successful initialization
		return true;
	}

	void FadeIn(double freq, double time)
	{
		double sampleRate = Generator.MixRate;
		for (double i = 0; i < time; i += 1 / sampleRate) 
		{
			if (Playback.GetFramesAvailable() == 0)
				break; // the buffer is less than the fade in time; that's ridiculously low

			float volume = (float)(i / time);

			Vector2 frame = volume * CreateSinWaveFrame(freq, sampleRate);
			Playback.PushFrame(frame);
		}
	}

	void FadeOut(double freq, double time)
	{
		double sampleRate = Generator.MixRate;
		for (double i = 0; i < time; i += 1 / sampleRate)
		{
			if (Playback.GetFramesAvailable() == 0)
				break; // the buffer is less than the fade in time; that's ridiculously low

			float volume = (float)(1 - (i / time));

			Vector2 frame = volume * CreateSinWaveFrame(freq, sampleRate);
			Playback.PushFrame(frame);
		}
	}

	/// <summary>
	/// Pushes the current note from noteSequence[] into the buffer.
	/// </summary>
	private void Play()
	{
		if (Timer.ElapsedTime >= EndTime)
		{
			StopPlaying();
			return;
		}
		// Check to see if Function sonification should be stopped.
		GenerateNoteAudio();
	}

	public void Info()
	{
		GD.Print("\tText:" + TextRepresentation + "\tLaTex(" + FunctionAST.Latex + ")");
	}

	/// <summary>
	/// Saves all data elements needed to create a Function to a Godot Dictionary.
	/// </summary>
	/// <returns>Returns the Godot Dictionary that holds the required information.</returns>
	public Godot.Collections.Dictionary Save()
	{
		var res = new Godot.Collections.Dictionary {
			{ "TextRepresentation", TextRepresentation },
			{ "StartTime", StartTime },
			{ "EndTime", EndTime }
		};
		return res;
	}

	public override void _Process(double delta)
	{
		Timer.Tick(delta);
		Play();
	}
}

