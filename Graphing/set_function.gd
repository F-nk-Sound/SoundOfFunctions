extends VBoxContainer

@onready var func_render: FunctionRenderer = $FunctionRenderer
@onready var text: LineEdit = $HBoxContainer/LineEdit

func _on_button_pressed():
	func_render.functionStr = text.text
	func_render.queue_redraw()
