class_name ConfigButton extends Control

func _on_pressed() -> void:
	AudioManager.play_button_click()
	ClickEvents.config_button_pressed.emit()

func _on_mouse_entered() -> void:
	AudioManager.play_button_hover()
