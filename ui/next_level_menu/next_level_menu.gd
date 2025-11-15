class_name NextLevelMenu extends Control

signal next_leve_button_pressed

func _on_button_pressed() -> void:
	next_leve_button_pressed.emit()
