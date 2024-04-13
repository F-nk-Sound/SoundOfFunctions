class_name FunctionRenderer
extends ColorRect

@export var functionStr: String = "sin(x)"

var function_ast

func f_gd(x: float) -> float:
	var expression: Expression = Expression.new()
	expression.parse(functionStr, ["x"])
	var result = expression.execute([x])
	if not (result is float or result is int):
		return INF
	return result


func f(x: float) -> float:
	return function_ast.EvaluateAtT(x)

func _draw():
	if function_ast:
		var scale: float = 100
		
		var extents: Rect2
		extents.size = size
		extents.position = position
		
		for x in range(extents.position.x, extents.end.x):
			var y: float = (extents.end.y/2-f((x - extents.end.x/2) / scale) * scale)
			var nextY: float = (extents.end.y/2-f((x+1 - extents.end.x/2) / scale) * scale)

			if y < extents.end.y * 2 and y > -extents.end.y:
				draw_line(Vector2(x,y), Vector2(x+1, nextY), Color.BLACK, 2, true)
