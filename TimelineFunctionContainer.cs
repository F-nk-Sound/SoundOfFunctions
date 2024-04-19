using System.Linq;
using Godot;
using Sonification;

public partial class TimelineFunctionContainer : Control
{
	[Export]
	Label? startLabel;

	[Export]
	Label? endLabel;

	[Export]
	LaTeXButton? latex;

	public int Index { get; set; }

	public LowerTimeline? Timeline { get; set; }

	public int StartTime
	{
		set
		{
			startLabel!.Text = value.ToString();
		}
	}

	public int EndTime
	{
		set
		{
			endLabel!.Text = value.ToString();
		}
	}

	public string LatexString
	{
		set
		{
			latex!.LatexExpression = value;
			latex.QueueRedraw();
		}
	}

	void Delete()
	{
		foreach (TimelineFunctionContainer child in GetParent().GetChildren().Cast<TimelineFunctionContainer>())
		{
			if (child.Index > Index)
			{
				child.Index -= 1;
			}
		}
		Timeline!.RemoveFunction(Index);
		QueueFree();
	}
}
