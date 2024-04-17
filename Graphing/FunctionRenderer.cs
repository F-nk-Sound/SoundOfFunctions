using Godot;
using System;
using Functions;
using Parsing;

namespace Graphing;

public partial class FunctionRenderer : ColorRect {
	public IFunctionAST ast = null;
	
	double f(double x) {
		return ast.EvaluateAtT(x);
	}
	
	public void SetFunction(IFunctionAST func) {
		ast = func;
		QueueRedraw();
	}
	
	public void ParseAndSetFunction(String toParse) {
		SetFunction(Bridge.Parse(toParse).Unwrap());
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
