using Godot;
using System;

public partial class Timeline : Node2D
{
	private HBoxContainer _container;
	private FunctionPalette functionPalette; // I believe I'm gonna need to reference the functionPalette's drag and drop
	private PackedScene? _timelineContainer;
	private int rectSize = 10;
	private int coord2 = 100;
	private int containerSizex = 80;
	private int containerSizey = 70;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready(); // I saw Mark had this
		ScrollContainer scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
		_container = scrollContainer.GetNode<HBoxContainer>("TimelineContainer");
		_timelineContainer = GD.Load<PackedScene>("res://UI/TimelineContainer.tscn");

		// Connect to the FunctionDragged signal
		// GetParent().GetNode<FunctionPalette>("Function Palette").FunctionDragged += OnFunctionDragged; 
		GetParent().GetNode<FunctionPalette>("Function Palette").FunctionDragged += AddToTimeline; 
	}

	public void AddToTimeline(Vector2 v)
	{
		if (_container == null || _timelineContainer == null) return;

		Container sizeContainer = new Container();
		sizeContainer.CustomMinimumSize = new Vector2(containerSizex, containerSizey);
		// var TimelineContainer = GD.Load<PackedScene>("res://UI/TimelineContainer.tscn");
		// var instance = _timelineContainer.Instantiate();
		Control instance = (Control) _timelineContainer.Instantiate();
		sizeContainer.AddChild(instance);
		_container.AddChild(sizeContainer);
		//_container.AddChild(instance)

		// ColorRect colorRect = GetNode<ColorRect>("ColorRect");
		// colorRect.Visible = false;

		//Vector2 colorRectSize = new Vector2(225, 70); // Coords 1
		Vector2 colorRectSize = new Vector2(coord2, coord2); // Coords 2

		// Create a new ColorRect instance
		ColorRect colorRect = new ColorRect();
		colorRect.Size = colorRectSize;
		colorRect.Color = Colors.Blue; // Example color
		
		// Position the ColorRect objects within the ScrollContainer
		//Vector2 position = new Vector2(28, 545 + (colorRectSize.y + 5)); // Adjust Y position based on index and size THIS IS FROM CHAT
		//Vector2 position = new Vector2(28, 545 + 70); // Coords 1
		Vector2 position = new Vector2(rectSize, rectSize + rectSize); // Coords 2
		colorRect.Position = position;

		// Add the ColorRect to the ScrollContainer
		_container.AddChild(colorRect);
	}

	public void OnFunctionDragged(Vector2 position)
	{
		// Logic to execute when the function is dragged
		GD.Print("Function dragged");
	}
}
