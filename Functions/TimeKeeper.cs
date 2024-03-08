using System;
using Godot;

namespace Functions;

public partial class TimeKeeper : Node {
    private double clock;				// Tracks time from 0 onwards (in seconds)
    private double elapsedTime;			// Tracks time between last timer call and current time
    private int lastTime;				// Previous rounded clock reading
    private int thisTime;				// Current rounded clock reading
    private double trackingStartTime;	// Time at which tracking began
    private bool isTracking;			// is tracking the elapsed time active
    private bool isPaused;				// Is the timer paused

    public TimeKeeper() {
        clock = 0.0;
        elapsedTime = 0.0;
        lastTime = 0;
        thisTime = 0;
        trackingStartTime = 0.0;
        isTracking = false;
        isPaused = false;
    }

    public bool Tick(double delta) {
        // Don't increment if timer is paused
        if(!isPaused) {
            // Increment the clock, elapsed time if needed
            clock += delta;
            if(isTracking) elapsedTime = clock - trackingStartTime;
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
        lastTime = 0;
        thisTime = 0;
        trackingStartTime = 0.0;
        isTracking = false;
        isPaused = false;
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

    public double GetElapsedTime() {
        return elapsedTime;
    }

    public bool GetIsTracking() {
        return isTracking;
    }

    public double GetCurrentTimeAbsolute() {
        return clock;
    }

    public int GetCurrentTimeRoundedDown() {
        int currTime;

        thisTime = (int) clock;
        if((int) lastTime != (int) thisTime) currTime = (int) thisTime;
        else currTime = (int) lastTime;

        lastTime = thisTime;
        return currTime;
    }

}