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

using Functions;
namespace Sonification;

/// <summary>
/// Main Audio Generation Node. 
/// </summary>
public partial class AudioGenerator : Node {

	/// <summary>
	/// LowerTimeline representation of the Timeline UI Node
	/// </summary>
	public LowerTimeline timeline;
	
	/// <summary>
	/// Timer to manage and synchronize audio playback within the AudioGenerator. 
	/// </summary>
	private TimeKeeper timer;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {

		Initialize();
		AddTests();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		
		// Increment the timer
		timer.Tick(delta);
		int currTime = timer.ClockTimeRounded;
		if(timer.IsTimeChanged) GD.Print("AudioGenerator Time t = " + currTime + " s");

		// Actually play functions
		if(currTime == 5 && !timeline.IsPlaying) timeline.StartPlaying();
		if(currTime > timeline.RunTime && !timeline.IsPlaying) {
			GD.Print("--------------------------------------------");
			PrintTreePretty();	// Just to signify the end for now.
			GetTree().Quit();
		}
		
	}

	/// <summary>
	/// Initializes the Audio Generator.
	/// </summary>
	public void Initialize() {
		timer = new TimeKeeper();
		timeline = new LowerTimeline(this);
	}

	public void AddTests() {
		IFunctionAST t = new Variable("t");
		IFunctionAST two = new Number(2);
	 	IFunctionAST three = new Number(3);
		IFunctionAST four = new Number(4);
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

		//Function threeFunc = new Function("3", three);
		Function inverseFunc = new Function("2/t)", inverse);
		Function sineFunc2 = new Function("Sin(3*t)", sin_3t);
		Function sineFunc1 = new Function("sin(3*pi*t)", sin_3_pi_t);
		Function linearFunc = new Function("3t + 4", three_t_plus_four);
		Function squareFunc = new Function("t^2", t_squared);
		Function polynomialFunc = new Function("t^2 + 3t + 4", poly);
		
		//timeline.Add(threeFunc);
		//timeline.Add(inverseFunc);
		timeline.Add(polynomialFunc);
		timeline.Add(squareFunc);
		timeline.Add(linearFunc);
		timeline.Add(sineFunc1);
		timeline.Add(sineFunc2);
		
	}
}
