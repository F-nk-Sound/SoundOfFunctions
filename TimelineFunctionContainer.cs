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
	private Theme? pauseTheme = null;

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

	private void OnFunctionPlaybackStarted() 
	{
		// Grab current theme for later.
		//pauseTheme ??= panel!.Theme;
		GD.Print("Case 1");
		// Load playing theme.
		panel!.Theme = (Theme) GD.Load("res://FunctionPlaying.tres");
	}

	private void OnFunctionPlaybackEnded() 
	{
		// Load paused theme.
		GD.Print("Case 2");
		panel!.Theme = pauseTheme;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		//pauseTheme ??= panel!.Theme;
		
		if(pauseTheme == null) {
			pauseTheme = panel!.Theme;
			GD.Print("Not Null anymore");
		}

		if(Function!.IsProcessing() && panel!.Theme != pauseTheme) OnFunctionPlaybackStarted();
		else if(!Function!.IsProcessing()) OnFunctionPlaybackEnded();
	}
}
