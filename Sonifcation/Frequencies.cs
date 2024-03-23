
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
    private static int tuningKeyNum = 49;						

    /// <summary>
    /// Tuning frequency (in Hz) corresponding tuning key. Default = A4 = 440 Hz.
    /// </summary>
    private static double tuningFreq = 440.0;	

    /// <summary>
    /// Number of possible notes.
    /// </summary>
    public static int resolution = 88;	

    /// <summary>
    /// Lookup table of note to frequency mappings basedon an 88-key piano.
    /// </summary>
    public static Dictionary<int, double> Notes = new Dictionary<int, double>(resolution);		

    static Frequencies() {
        FillDictionary();	
    }

    /// <summary>
    /// Fills the note to frequency lookup table via formula found from wikipedia.
    /// </summary>
    static void FillDictionary() {
        for(double n = 1; n <= resolution; n++) {
            double freq = Math.Pow(2.0, (n - tuningKeyNum) / 12.0) * tuningFreq;
            Notes.Add((int) n, freq);
        }
    }

}