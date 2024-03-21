using System;
using Godot;

namespace Functions;

public partial class TimeKeeper {

    /// <summary>
    /// Current time TimeKeeper has kept time for since construction (in seconds).
    /// </summary>
    public double ClockTimeAbsolute {get; set;}			

    /// <summary>
    /// Current time TimeKeeper has kept time for since construction rounded down to the nearest second (in seconds).
    /// </summary>
    public int ClockTimeRounded {get; set;}       

    /// <summary>
    /// If <c> true </c>, TimeKeeper has advanced to the next second.
    /// </summary>
    public bool IsTimeChanged {get; set;}        

    /// <summary>
    /// If <c> true </c>, TimeKeeper is currently in tracking mode. 
    /// See <c> TimeKeeper.ElapsedTime </c> to retrieve the time it has been tracking for.
    /// </summary>
    public bool IsTracking {get; set;}			

    /// <summary>
    /// Current time TimeKeeper has kept time put into Tracking Mode (in seconds).
    /// </summary>
    public double ElapsedTime {get; set;}	

    /// <summary>
    /// Time at which TimeKeeper activated Tracking Mode (in seconds).
    /// </summary>
    private double trackingStartTime;	

    /// <summary>
    /// Previous TimeKeeper <c> ClockRounded </c> reading for internal calculations (in seconds).
    /// </summary>
    private int lastTime;				

    /// <summary>
    /// Current TimeKeeper <c> ClockRounded </c> reading for internal calculations (in seconds).
    /// </summary>
    private int thisTime;				

    /// <summary>
    /// If <c> true </c>, TimeKeeper is currently not updating.
    /// </summary>
    private bool isPaused;				

    /// <summary>
    /// Initializes a new TimeKeeper Node that begins timing at t = 0 seconds.
    /// </summary>
    public TimeKeeper() {
        ClockTimeAbsolute = 0.0;
        ElapsedTime = 0.0;
        lastTime = -1;
        thisTime = -1;
        trackingStartTime = 0.0;
        IsTracking = false;
        isPaused = false;
        IsTimeChanged = false;
    }

    /// <summary>
    /// Initializes a new TimeKeeper Node that begins timing at t = <c>startTime</c> seconds.
    /// </summary>
    /// <param name="startTime">The time at which this TimeKeeper counts up from.</param>
    public TimeKeeper(double startTime) {
        ClockTimeAbsolute = startTime;
        ElapsedTime = 0.0;
        lastTime = -1;
        thisTime = -1;
        trackingStartTime = 0.0;
        IsTracking = false;
        isPaused = false;
        IsTimeChanged = false;
    }

    /// <summary>
    /// Increment TimeKeeper by <c>delta</c> seconds.
    /// </summary>
    /// <param name="delta">Amount of time to increment the TimeKeeper by (in seconds).</param>
    /// <returns> <c>true</c> if TimeKeeper was succesfully incremented.</returns>
    public bool Tick(double delta) {
        // Don't increment if timer is paused
        if(!isPaused) {
            // Increment the ClockTimeAbsolute, elapsed time if needed
            ClockTimeAbsolute += delta;
            if(IsTracking) ElapsedTime = ClockTimeAbsolute - trackingStartTime;
            ClockTimeRounded = GetCurrentTimeRoundedDown();
        }	
        return !isPaused;
    }

    /// <summary>
    /// Stops the TimeKeeper from ticking until <c> UnPause() </c> is called.
    /// </summary>
    /// <exception cref="Exception"> Thrown if an attempt to Pause the TimeKeeper was made while the TimeKeeper is in Tracking Mode.</exception>
    public void Pause() {
        // Don't allow pause in middle of tracking
        if(IsTracking) throw new Exception("AttemptedPauseWhileTracking");
        else isPaused = true;
    }

    /// <summary>
    /// Restarts the TimeKeeper after <c> Pause() </c> was called.
    /// </summary>
    /// <exception cref="Exception">Thrown if an attempt to UnPause the TimeKeeper was made while the TimeKeeper is already UnPaused.</exception>
    public void UnPause() {
        // Don't allow unpausing of an unpaused timer
        if(!isPaused) throw new Exception("AttemptedUnpausingOfAnUnpausedTimer");
        else isPaused = false;
    }
    
    /// <summary>
    /// Resets the TimeKeeper to default settings and begins counting from 0 seconds.
    /// </summary>
    public void Reset() {
        ClockTimeAbsolute = 0.0;
        ElapsedTime = 0.0;
        lastTime = -1;
        thisTime = -1;
        trackingStartTime = 0.0;
        IsTracking = false;
        isPaused = false;
        IsTimeChanged = false;
    }

    /// <summary>
    /// Enables the TimeKeepers Tracking Mode.
    /// </summary>
    /// <exception cref="Exception">Thrown if an attempt to Enable Tracking Mode happens while the TimeKeeper is paused.</exception>
    public void BeginTracking() {
        // Don't allow tracking to begin while paused
        if(isPaused) throw new Exception("AttmeptedTrackingWhilePaused");
        else {
            IsTracking = true;
            trackingStartTime = ClockTimeAbsolute;
        }
    }

    /// <summary>
    /// Resets Tracking Mode. TimeKeeper begins tracking anew at the time which this method is called.
    /// </summary>
    public void ResetTracking() {
        ElapsedTime = 0.0;
        trackingStartTime = ClockTimeRounded;
    }

    /// <summary>
    /// Disables TimeKeeper Tracking Mode.
    /// </summary>
    /// <exception cref="Exception">Thrown if an attempt to disable Tracking Mode happends while the TimeKeeper is unpaused.</exception>
    public void EndTracking() {
        // Don't allow tracking to stop for a tracker that's stopped
        if(!IsTracking) throw new Exception("AttemptedToEndTrackingForNonTrackingTimer");
        else {
            IsTracking = false;
            ElapsedTime = 0.0;
        }
    }
    
    /// <summary>
    /// Computes the current count of the TimeKeeper rounded down to the nearest second based on the current value
    /// of <c> ClockAbsolute </c> and its most recent past value. Called alongside <c> Tick() </c> and uses 
    /// the two values to determine if the rounded time has changed since the last call.
    /// </summary>
    /// <returns>The value of the current time rounded down to the nearest second.</returns>
    private int GetCurrentTimeRoundedDown() {

        // Truncate the current ClockTimeAbsolute time
        int currTime;
        thisTime = (int) ClockTimeAbsolute;

        // Track if the time has changed since the last tick
        if((int) lastTime != (int) thisTime) {
            currTime = (int) thisTime;
            IsTimeChanged = true;
        }
        else {
            currTime = (int) lastTime;
            IsTimeChanged  = false;
        }

        // Update the last time to the current for future calls
        lastTime = thisTime;
        return currTime;
    }

}