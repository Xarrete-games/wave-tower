class_name NextWaveScreen extends Control

func _on_next_wave_button_pressed() -> void:
	ClickEvents.emit_signal("next_wave_pressed")
	queue_free()
