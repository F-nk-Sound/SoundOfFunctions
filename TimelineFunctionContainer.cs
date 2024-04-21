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
	private StyleBox? pausedBox = (StyleBox) GD.Load("res://FunctionPaused.tres");
	private StyleBox? playingBox = (StyleBox) GD.Load("res://FunctionPlaying.tres");

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

	public void Initialize(Function func, LowerTimeline timeline)
	{
		Function = func;
		startLabel!.Text = func.StartTime.ToString();
		endLabel!.Text = func.EndTime.ToString();
		LatexString = func.FunctionAST!.Latex;
		Timeline = timeline;
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

	private void ChangePausedToPlaying() 
	{
		// Load playing theme.
		GD.Print("Case 1");
		panel!.RemoveThemeStyleboxOverride("Paused");
		panel!.AddThemeStyleboxOverride("panel", playingBox);
	}

	private void ChangePlayingToPaused() 
	{
		// Load paused theme.
		GD.Print("Case 2");
		panel!.RemoveThemeStyleboxOverride("Playing");
		panel!.AddThemeStyleboxOverride("panel", pausedBox);
	}

	public override void _Process(double delta)
	{
		StyleBox box = panel!.GetThemeStylebox("panel");
		if(Function!.IsProcessing() && box.ResourceName.Equals("Paused")) ChangePausedToPlaying();
		else if(!Function!.IsProcessing() && box.ResourceName.Equals("Playing")) ChangePlayingToPaused();
	}
}
