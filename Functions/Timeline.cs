
using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Functions;
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
            //Task.Delay(1000 * func.length());
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