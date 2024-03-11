using System;
using Godot;

namespace Functions;

public partial class TimeKeeper : Node {
    private double clock;				// Tracks time from 0 onwards (in seconds)
    private int roundedClock;           // Clock value rounded down
    private double elapsedTime;			// Tracks time between last timer call and current time
    private int lastTime;				// Previous rounded clock reading
    private int thisTime;				// Current rounded clock reading
    private double trackingStartTime;	// Time at which tracking began
    private bool isTracking;			// is tracking the elapsed time active
    private bool isPaused;				// Is the timer paused
    private bool isTimeChanged;         // Has the timer advanced to a new second

    public TimeKeeper() {
        clock = 0.0;
        elapsedTime = 0.0;
        lastTime = -1;
        thisTime = -1;
        trackingStartTime = 0.0;
        isTracking = false;
        isPaused = false;
        isTimeChanged = false;
    }

    public bool Tick(double delta) {
        // Don't increment if timer is paused
        if(!isPaused) {
            // Increment the clock, elapsed time if needed
            clock += delta;
            if(isTracking) elapsedTime = clock - trackingStartTime;
            roundedClock = GetCurrentTimeRoundedDown();
        }	
        return !isPaused;
    }

    public void Pause() {
        // Don't allow pause in middle of tracking
        if(isTracking) throw new Exception("AttemptedPauseWhileTracking");
        else isPaused = true;
    }

    public void UnPause() {
        // Don't allow unpausing of an unpaused timer
        if(!isPaused) throw new Exception("AttemptedUnpausingOfAnUnpausedTimer");
        else isPaused = false;
    }
    
    public void Reset() {
        clock = 0.0;
        elapsedTime = 0.0;
        lastTime = -1;
        thisTime = -1;
        trackingStartTime = 0.0;
        isTracking = false;
        isPaused = false;
        isTimeChanged = false;
    }

    public void BeginTracking() {
        // Don't allow tracking to begin while paused
        if(isPaused) throw new Exception("AttmeptedTrackingWhilePaused");
        else {
            isTracking = true;
            trackingStartTime = clock;
        }
    }

    public void EndTracking() {
        // Don't allow tracking to stop for a tracker that's stopped
        if(!isTracking) throw new Exception("AttemptedToEndTrackingForNonTrackingTimer");
        else {
            isTracking = false;
            elapsedTime = 0.0;
        }
    }

    public bool GetTimeChanged() {
        return isTimeChanged;
	}

    public double GetElapsedTime() {
        return elapsedTime;
    }

    public bool GetIsTracking() {
        return isTracking;
    }

    public double GetCurrentTimeAbsolute() {
        return clock;
    }

    public int GetRoundedClockTime() {
        return roundedClock;
    }
    
    private int GetCurrentTimeRoundedDown() {

        // Truncate the current clock time
        int currTime;
        thisTime = (int) clock;

        // Track if the time has changed since the last tick
        if((int) lastTime != (int) thisTime) {
            currTime = (int) thisTime;
            isTimeChanged = true;
        }
        else {
            currTime = (int) lastTime;
            isTimeChanged  = false;
        }

        // Update the last time to the current for future calls
        lastTime = thisTime;
        return currTime;
    }

}