
using System;   
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Functions;
public partial class Function : Node {
    private string textRepresentation;		// Input text funtion is parsed from
    private string type;					// Type of Function: i.e., Cubic, Logarithimic, Linear
    private int runLength;					// Size of domain	
    private int startPos;					// Starting time point
    private int endPos;						// Ending time point
    private List<int> timeDomain;			// Function domain: [startPos, ..., endPos]
    private List<int> noteSequence;			// Frequency domain: [minFreq, ..., maxFreq]
    private AudioStreamPlayer player;		// Audio Player
    public bool isPlaying;			        // Duh

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

    public override void _Process(double delta) {
        GD.Print("This is From the Function Class");
    }
}
