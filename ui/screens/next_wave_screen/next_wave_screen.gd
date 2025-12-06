class_name NextWaveScreen extends Control

signal button_pressed()

func _on_next_wave_button_xarreta_pressed() -> void:
	button_pressed.emit()
	queue_free()
