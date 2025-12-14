class_name NextLevelScreen extends Control

func _on_next_level_button_pressed() -> void:
	ClickEvents.next_level_pressed.emit()
	queue_free()
