class_name NextLevelScreen extends Control

signal button_pressed()

func _on_next_level_button_pressed() -> void:
	button_pressed.emit()
	queue_free()
