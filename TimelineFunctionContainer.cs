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

	[Export]
	PanelContainer? panel;

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
		if(Timeline!.IsPlaying) return;
		QueueFree();
	}

  public override Variant _GetDragData(Vector2 atPosition)
  {
		SetDragPreview((Control)Duplicate());
		QueueFree();
		return Function!;
  }
}
