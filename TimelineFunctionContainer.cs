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

	public LowerTimeline? Timeline { get; set; }

	public Function? Function { get; set; }

	public string LatexString
	{
		set
		{
			latex!.LatexExpression = value;
			latex.QueueRedraw();
		}
	}

	public void Initialize(Function func)
	{
		Function = func;
		startLabel!.Text = func.StartTime.ToString();
		endLabel!.Text = func.EndTime.ToString();
		LatexString = func.FunctionAST!.Latex;
	}

	void Delete()
	{
		QueueFree();
	}

    public override Variant _GetDragData(Vector2 atPosition)
    {
		SetDragPreview((Control)Duplicate());
		QueueFree();
		return Function!;
    }
}
