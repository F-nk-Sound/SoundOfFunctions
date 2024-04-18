using Godot;
using Functions;
using Sonification;

namespace Graphing;

public partial class FunctionRenderer : ColorRect 
{
	public IFunctionAST? ast = null;
	
	double F(double x) 
	{
		return ast!.EvaluateAtT(x);
	}
	
	public void SetFunction(Function func) 
	{
		ast = func.FunctionAST;
		QueueRedraw();
	}
	
	public override void _Draw() 
	{
		if (ast != null) 
		{
			float scale = 100;
            Rect2 extents = new()
            {
                Size = Size,
                Position = Position
            };

            for (float x = extents.Position.X; x < extents.End.X; x++) {
				float y = (float)(extents.End.Y/2-F((x - extents.End.X/2) / scale) * scale);
				float nextY = (float)(extents.End.Y/2-F((x+1 - extents.End.X/2) / scale) * scale);
				
				if (y < extents.End.Y * 2 && y > -extents.End.Y) {
					DrawLine(new Vector2(x,y), new Vector2(x+1, nextY), new Color(0,0,0,255), 2, true);
				}
			}
		}
	}
}
