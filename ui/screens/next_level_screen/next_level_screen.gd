class_name NextLevelScreen extends Control

func _on_next_level_button_pressed() -> void:
	ClickEvents.emit_next_level_pressed()
	queue_free()
