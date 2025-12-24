class_name NextWaveScreen extends Control

func _on_next_wave_button_pressed() -> void:
	ClickEvents.next_wave_pressed.emit()
	queue_free()
