
using System;
using System.Collections.Generic;

namespace Sonification;

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
    private static readonly int tuningKeyNum = 49;						

    /// <summary>
    /// Tuning frequency (in Hz) corresponding tuning key. Default = A4 = 440 Hz.
    /// </summary>
    private static readonly double tuningFreq = 440.0;	

    /// <summary>
    /// Note number that audio starts at.
    /// </summary>
    public static readonly int startingNoteNumber = 13;

    /// <summary>
    /// Note number that audio ends at.
    /// </summary>
    public static readonly int endingNoteNumber = 88;

    /// <summary>
    /// Lookup table of note to frequency mappings basedon an 88-key piano.
    /// </summary>
    public static readonly Dictionary<int, double> Notes = InitializeNotes();		

    /// <summary>
    /// Fills the note to frequency lookup table via formula found from wikipedia.
    /// </summary>
    private static Dictionary<int,double> InitializeNotes() {
        Dictionary<int, double> res =  new Dictionary<int, double>(endingNoteNumber - startingNoteNumber + 1);
        res.Add(0,0);
        for(double n = startingNoteNumber; n <= endingNoteNumber; n++) {
            double freq = Math.Pow(2.0, (n - tuningKeyNum) / 12.0) * tuningFreq;
            res.Add((int) n, freq);
        }
        return res;
    }

}