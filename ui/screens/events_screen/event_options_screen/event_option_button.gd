class_name EventOptionButton extends Button

signal option_selected(data: Variant)

var option_data: Variant

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()

func _on_pressed() -> void:
	AudioManager.play_button_click()
	option_selected.emit(option_data)
