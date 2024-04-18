using Godot;
using Functions;
using Sonification;

namespace Graphing;

public partial class FunctionRenderer : ColorRect
{
	public IFunctionAST? ast = null;

	Vector2 originOffset;
	float PixelsPerUnit { get; set; } = 32;

	public override void _Ready()
	{
		ResetOrigin();
	}

	void OnResetButton()
	{
		ResetOrigin();
		PixelsPerUnit = 32;
		QueueRedraw();
	}

	double F(double x)
	{
		return ast!.EvaluateAtT(x);
	}

	public void SetFunction(Function func)
	{
		ast = func.FunctionAST;
		ResetOrigin();
		QueueRedraw();
	}

	public override void _Draw()
	{
		// Graph axes
		DrawLine(new(XCoordToLine(0), 0), new(XCoordToLine(0), Size.Y), Colors.LightGray, -1, true);
		DrawLine(new(0, YCoordToLine(0)), new(Size.X, YCoordToLine(0)), Colors.LightGray, -1, true);

		if (ast != null)
		{
			for (float x = 0; x < Size.X; x++)
			{
				float xCoord = LineToXCoord(x);
				float nextXCoord = LineToXCoord(x + 1);

				float yCoord = (float)F(xCoord);
				float y = YCoordToLine(yCoord);

				float nextYCoord = (float)F(nextXCoord);
				float nextY = YCoordToLine(nextYCoord);

				DrawLine(new Vector2(x, y), new Vector2(x + 1, nextY), new Color(0, 0, 0, 255), -1, true);
			}
		}
	}

	float XCoordToLine(float xCoord) => (xCoord * PixelsPerUnit) + originOffset.X;
	// 0 means "top" for the line, so we have to flip the coordinates vertically
	float YCoordToLine(float yCoord) => (yCoord * -PixelsPerUnit) + originOffset.Y;
	Vector2 CoordsToLine(Vector2 coords) => new(XCoordToLine(coords.X), YCoordToLine(coords.Y));

	float LineToXCoord(float line) => (line - originOffset.X) / PixelsPerUnit;
	float LineToYCoord(float line) => (line - originOffset.Y) / -PixelsPerUnit;
	Vector2 LineToCoords(Vector2 line) => new(LineToXCoord(line.X), LineToYCoord(line.Y));

	void ResetOrigin()
	{
		originOffset = new(Size.X / 2, Size.Y / 2);
	}

	bool panning = false;
	Vector2? oldOffset = null;
	Vector2? panStart = null;
	Vector2? panCurrent = null;

	public override void _GuiInput(InputEvent ev)
	{
		switch (ev)
		{
			case InputEventMouseButton mb:
				if (!panning && mb.IsPressed() && mb.ButtonIndex == MouseButton.Left)
				{
					panning = true;
					panStart = panCurrent = mb.Position;
					oldOffset = originOffset;
				}
				if (panning && mb.IsReleased() && mb.ButtonIndex == MouseButton.Left)
				{
					panning = false;
					panStart = panCurrent = null;
					oldOffset = null;
				}
				if (!panning && mb.ButtonIndex == MouseButton.WheelUp)
				{
					Vector2 mouseCoord = LineToCoords(mb.Position);

					PixelsPerUnit *= 1.1f;

					if (PixelsPerUnit == float.PositiveInfinity)
						PixelsPerUnit = float.MaxValue;

					Vector2 newLinePos = CoordsToLine(mouseCoord);
					Vector2 offset = newLinePos - mb.Position;

					originOffset -= offset;

					QueueRedraw();
				}
				if (!panning && mb.ButtonIndex == MouseButton.WheelDown)
				{
					Vector2 mouseCoord = LineToCoords(mb.Position);

					PixelsPerUnit /= 1.1f;

					// limit chosen to be far away from the point where dividing by PPU produces 
					// infinity
					if (PixelsPerUnit < 1e-40)
						PixelsPerUnit = 1e-40f;

					Vector2 newLinePos = CoordsToLine(mouseCoord);
					Vector2 offset = newLinePos - mb.Position;

					originOffset -= offset;

					QueueRedraw();
				}
				break;
			case InputEventMouseMotion mm:
				if (panning)
				{
					panCurrent = mm.Position;
				}
				break;
		}
	}

	bool initialReset = false;

	public override void _Process(double delta)
	{
		if (!initialReset)
		{
			ResetOrigin();
			QueueRedraw();
			initialReset = true;
		}

		if (panning)
		{
			Vector2 offset = panCurrent!.Value - panStart!.Value;

			originOffset = oldOffset!.Value + offset;

			QueueRedraw();
		}
	}
}
