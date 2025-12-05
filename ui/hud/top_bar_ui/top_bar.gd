class_name TopBar extends MarginContainer

signal config_pressed()

@export var speed_button: XarretaButton

func _ready() -> void:
	speed_button.text = _build_speed_button_text(int(Engine.time_scale))
	
func _on_config_config_pressed() -> void:
	config_pressed.emit()
	
func _on_xarreta_text_button_xarreta_pressed() -> void:
	if Engine.time_scale == 1.0:
		Engine.time_scale = 2.0
	elif Engine.time_scale == 2.0:
		Engine.time_scale = 3.0
	elif Engine.time_scale == 3.0:
		Engine.time_scale = 1.0
	speed_button.text = _build_speed_button_text(int(Engine.time_scale))
	
func _build_speed_button_text(value: int) -> String:
	return "x" + str(value)
