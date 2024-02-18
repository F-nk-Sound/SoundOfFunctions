using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public partial class TextUpdate : TextEdit
{
	LaTeX latex;
	double timeBetween = 0.75;
	double total = 0.0;
	bool active = true;

	private void make()
	{
		latex.Render();
		Vector2 size;
		//Math.Max(this.Position.Y / 2 + latex.Height / 2, 30);
		size = new Vector2(Math.Max(this.Position.X / 2 + latex.Width / 2, 225), Math.Max(this.Position.Y / 2 + latex.Height / 2, 60));
		this.Size = size;
		latex.Position = new Vector2(latex.Width / 2, size.Y / 2);
		// this.Position.X / 2 + size.X / 2
		latex.Render();
		
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Node node;
		node = this.GetParent<Node>();
		foreach(Node child in node.GetChildren())
		{
			if (child.Name == "LaTeX")
			{
				latex = (LaTeX) child;
				break;
			}
		}

		this.OnTextChanged();
	}

	public override void _Process(double delta)
	{
		total += delta;
		if (total > timeBetween)
		{
			total = 0.0;
			active = !active;
		}
	}
	
	private void OnTextChanged()
	{
		String newText = this.Text;
		if (newText.Equals(""))
			latex.LatexExpression = this.PlaceholderText;
		else
			latex.LatexExpression = newText;

		this.make();
	}
}
