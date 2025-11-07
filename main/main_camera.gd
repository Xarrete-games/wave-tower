class_name MainCamera extends Camera2D

@export var move_speed := 400.0 
@export var zoom_speed := 0.1
@export var min_zoom := Vector2(0.5, 0.5)
@export var max_zoom := Vector2(2.0, 2.0)

func _process(delta: float) -> void:
	var input_vector = Vector2.ZERO

	input_vector.x = Input.get_action_strength("move_right") - Input.get_action_strength("move_left")
	input_vector.y = Input.get_action_strength("move_down") - Input.get_action_strength("move_up")

	if input_vector != Vector2.ZERO:
		input_vector = input_vector.normalized()

	global_position += input_vector * move_speed * delta

func _unhandled_input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.is_pressed():
			zoom = (zoom - Vector2(zoom_speed, zoom_speed)).clamp(min_zoom, max_zoom)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.is_pressed():
			zoom = (zoom + Vector2(zoom_speed, zoom_speed)).clamp(min_zoom, max_zoom)
