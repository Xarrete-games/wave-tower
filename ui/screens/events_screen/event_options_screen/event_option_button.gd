class_name EventOptionButton extends Button

signal option_selected(data: Variant)

var option_data: Variant

func disable_option() -> void:
	disabled = true
	self.modulate = Color(0.5, 0.5, 0.5)  # Grey out the button

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()

func _on_pressed() -> void:
	AudioManager.play_button_click()
	option_selected.emit(option_data)
