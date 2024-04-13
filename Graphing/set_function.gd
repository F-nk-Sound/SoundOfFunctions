extends VBoxContainer

@onready var func_render: FunctionRenderer = $FunctionRenderer
@onready var text: LineEdit = $HBoxContainer/LineEdit
@onready var ParseWrapper = load("res://Graphing/ParseWrapper.cs")

func _on_button_pressed():
	var parser = ParseWrapper.new()
	func_render.functionStr = text.text
	print(parser.getFive(text.text))
	func_render.function_ast = parser.ParseString(text.text)
	
	func_render.queue_redraw()
