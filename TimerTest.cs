using Godot;
using System;

using Functions;

public partial class Root : Node {

	double duration = 1.0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Create new Timeline
		Timeline tl = new Timeline();
		SetProcess(false);
	}

	readonly TimeKeeper timer = new TimeKeeper();
	int state = 0;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

		// Move forward a tick. Get the current time.
		timer.Tick(delta);
		int currTime = timer.GetRoundedClockTime();
		
		switch (state) {
			// TEST BLOCKING
			case 0:
				bool play1 = currTime >= 1 && currTime <= 3;
				bool play2 = currTime >= 5 && currTime <= 7;
				bool play3 = currTime >= 9 && currTime <= 11;
				
				if(play1 && !played1) {
					play(1, currTime);
					played1 = true;
				} 
				else if(play2 && !played2) {
					played2 = true;
					play(2, currTime);
				}
				else if(play3 && !played3) {
					played3 = true;
					play(3, currTime);
				}
				
				if(played3) {
					state = 1;
					timer.Reset();
				}
				
				break;

			// TEST ELAPSED TIME
			case 1:
				if(!timer.GetIsTracking() && currTime == 1) {
					GD.Print("Current Time : " + currTime);
					timer.BeginTracking();
				}
				if(timer.GetIsTracking() && currTime == 5) {
					GD.Print("Current Time: " + currTime);
					GD.Print("Elapsed Time should be ~4 seconds: Measured = " + timer.GetElapsedTime());
					timer.EndTracking();
				}

				if(currTime > 6.0 && !timer.GetIsTracking()) {
					timer.Reset();
					state = -1;
				}

				break;
			default:
				break;
		}
		

	}
	
	
	bool played1 = false, played2 = false, played3 = false;
	int funcDur = 10;
	void play(int n, double currTime) {
		GD.Print("playing " + n + " @ time: " + currTime);
	}
}


