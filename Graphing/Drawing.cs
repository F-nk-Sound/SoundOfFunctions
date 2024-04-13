using Godot;
using System;
using Functions;

public partial class Drawing : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
	
	public IFunctionAST ast = null;
	
	double f(double x) {
		return ast.EvaluateAtT(x);
	}
	
	public override void _Draw() {
		if (ast != null) {
			float scale = 100;
			Rect2 extents = new Rect2();
			
			extents.Size = Size;
			extents.Position = Position;
			
			for (float x = extents.Position.X; x < extents.End.X; x++) {
				float y = (float)(extents.End.Y/2-f((x - extents.End.X/2) / scale) * scale);
				float nextY = (float)(extents.End.Y/2-f((x+1 - extents.End.X/2) / scale) * scale);
				
				if (y < extents.End.Y * 2 && y > -extents.End.Y) {
					DrawLine(new Vector2(x,y), new Vector2(x+1, nextY), new Color(0,0,0,255), 2, true);
				}
			}
		}
	}
}
