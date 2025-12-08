class_name TopBar extends MarginContainer

@export var speed_button: XarretaButton

func _ready() -> void:
	_update_text(GameState.speed)
	GameState.speed_change.connect(_update_text)
	
func _on_xarreta_text_button_xarreta_pressed() -> void:
	ButtonsEvents.speed_button_pressed.emit()

func _update_text(value: float)-> void:
	speed_button.text = "x" + str(int(value))
