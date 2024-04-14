extends Control

var start : Vector2
var initialPosition : Vector2
var isMoving : bool
var isResizing : bool
var resizeX : bool
var initialSize : Vector2
@export var GrabThreshold := 20
@export var ResizeThreshold := 10

func _input(event):
	if Input.is_action_just_pressed("LeftMouseDown"):
		var rect = get_global_rect()
		var localMousePos = event.position - get_global_position()
		if localMousePos.y < GrabThreshold:
			start = event.position
			initialPosition = get_global_position()
			#isMoving = true
		else: # GET THIS
			if abs(localMousePos.x - rect.size.x) < ResizeThreshold: # GET THIS
				start.x = event.position.x # GET THIS
				initialSize.x = get_size().x # GET THIS
				resizeX = true # GET THIS
				isResizing = true # GET THIS
			
			#if abs(localMousePos.y - rect.size.y) < ResizeThreshold:
				#start.y = event.position.y
				#initialSize.y = get_size().y
				#resizeY = true
				#isResizing = true
			
			# if mouse position in the container, how close is it to the container
			if localMousePos.x < ResizeThreshold &&  localMousePos.x > -ResizeThreshold:
				# whereever our mouse position is, to know where we start
				start.x = event.position.x
				# where our mouse position on the object itself
				initialPosition.x = get_global_position().x
				# get the size of our object, so when we change it, we can offset/update those values later
				initialSize.x = get_size().x
				isResizing = true
				resizeX = true
				
			#if localMousePos.y < ResizeThreshold &&  localMousePos.y > -ResizeThreshold:
				#start.y = event.position.y
				#initialPosition.y = get_global_position().y
				#initialSize.y = get_size().y
				#isResizing = true
				#resizeY = true
		
	if Input.is_action_pressed("LeftMouseDown"):
		#if isMoving:
			#set_position(initialPosition + (event.position - start))
		
		if isResizing:
			var newWidith = get_size().x
			#var newHeight = get_size().y
			
			if resizeX:
				newWidith = initialSize.x - (start.x - event.position.x)
			#if resizeY:
				#newHeight = initialSize.y - (start.y - event.position.y)
				
			#if initialPosition.x != 0:
				#newWidith = initialSize.x + (start.x - event.position.x)
				#set_position(Vector2(initialPosition.x - (newWidith - initialSize.x), get_position().y))
			
			#if initialPosition.y != 0:
				#newHeight = initialSize.y + (start.y - event.position.y)
				#set_position(Vector2(get_position().x, initialPosition.y - (newHeight - initialSize.y)))
			
			set_size(Vector2(newWidith, get_size().y))
			
		
	if Input.is_action_just_released("LeftMouseDown"):
		isMoving = false
		initialPosition = Vector2(0,0)
		resizeX = false
		isResizing = false
