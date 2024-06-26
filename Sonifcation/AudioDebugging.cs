
using Godot;

namespace Sonification;

/// <summary>
/// For Debugging purposes. If <c> Enabled = true </c>, debug output related <br/>
/// to timing and playbak status will be enabled.
/// </summary>
public static class AudioDebugging
{
	public static readonly bool Enabled = false;
	public static void Output(string info)
	{
		if (Enabled) GD.Print(info);
	}
}
