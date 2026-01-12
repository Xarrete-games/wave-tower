class_name EventOptionButton extends Button

signal option_selected(option_index: int)

var option_index: int

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()

func _on_pressed() -> void:
	AudioManager.play_button_click()
	option_selected.emit(option_index)

