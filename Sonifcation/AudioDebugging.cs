
using Godot;

namespace Sonification;

/// <summary>
/// For Debugging purposes. If <c> Enabled = true </c>, debug output related <br/>
/// to timing and playbak status will be enabled.
/// </summary>
public static class AudioDebugging {
	private static readonly bool Enabled = false;
	public static readonly int Method = 1;
	public static readonly bool TestingJson = true;
	public static void Output(string info) {
		if(Enabled) GD.Print(info);
	}
}
