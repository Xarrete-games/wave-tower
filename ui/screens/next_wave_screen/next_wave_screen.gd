class_name NextWaveScreen extends Control

func _on_next_wave_button_xarreta_pressed() -> void:
	ButtonsEvents.next_wave_pressed.emit()
	queue_free()
