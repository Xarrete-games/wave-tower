class_name MainCamera extends Camera2D

@export var move_speed := 500.0
@export var zoom_tween_duration := 0.25
@export var level: Node2D

## Fixed zoom steps that look crisp on the tilemap.
const ZOOM_STEPS: Array[float] = [0.5, 0.75, 1.0]
var _zoom_index: int = 0
var _zoom_tween: Tween = null

func _ready() -> void:
	_zoom_index = 0
	zoom = Vector2(ZOOM_STEPS[_zoom_index], ZOOM_STEPS[_zoom_index])

func _process(delta: float) -> void:
	var input_vector = Vector2.ZERO

	input_vector.x = Input.get_action_strength("move_right") - Input.get_action_strength("move_left")
	input_vector.y = Input.get_action_strength("move_down") - Input.get_action_strength("move_up")

	if input_vector != Vector2.ZERO:
		input_vector = input_vector.normalized()

	global_position += input_vector * move_speed * delta

func _input(event: InputEvent) -> void:
	# for center camera (disabled)
	if event.is_action_pressed("test"):
		pass
		#global_position = level.global_position


func _unhandled_input(event):
	if event is InputEventMouseButton and event.is_pressed():
		if _zoom_tween and _zoom_tween.is_running():
			return
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			_change_zoom_step(1)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			_change_zoom_step(-1)


func _change_zoom_step(direction: int) -> void:
	var new_index: int = clampi(_zoom_index + direction, 0, ZOOM_STEPS.size() - 1)
	if new_index == _zoom_index:
		return
	_zoom_index = new_index

	var target: float = ZOOM_STEPS[_zoom_index]
	if _zoom_tween and _zoom_tween.is_running():
		_zoom_tween.kill()
	_zoom_tween = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC)
	_zoom_tween.tween_property(self, "zoom", Vector2(target, target), zoom_tween_duration)
