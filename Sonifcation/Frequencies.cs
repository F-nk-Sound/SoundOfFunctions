
using System;
using System.Collections.Generic;
using Godot;

namespace Sonification;

/// <summary>
/// Provides lookup table of note to frequency mappings. <br/>
/// Access Example : Frequencies.Notes[n] returns the frequency to which note n points to. <br/>
/// 'n' corresponds with the same 'n' in this table from wikipedia: <br/>
/// https://en.wikipedia.org/wiki/Piano_key_frequencies#List
/// </summary>
public static class Frequencies {
    /// <summary>
    /// Tuning frequency (in Hz) corresponding tuning key. Default = A4 = 440 Hz.
    /// </summary>
    public static readonly double tuningFreq = 440.0;

    /// <summary>
    /// Tuning key. Default = A4 = 49.
    /// </summary>
    public static readonly double tuningKey = 49;

    /// <summary>
    /// The key at y = 0. 
    /// Default is C4 = 40.
    /// </summary>
    static readonly double zeroKey = 40;

    /// <summary>
    /// Note number that audio starts at.
    /// </summary>
    public static readonly int firstKey = 1;

    /// <summary>
    /// Note number that audio ends at.
    /// </summary>
    public static readonly int lastKey = 88;

    public static double GetFrequency(double y)
    {
        double upperBound = Math.BitIncrement(lastKey - firstKey);
        double keyIndex = ((y + zeroKey - firstKey) % upperBound) + firstKey;
        double semitoneDistance = keyIndex - tuningKey;
        
        return Math.Pow(2.0, semitoneDistance / 12.0) * tuningFreq;
    }
}