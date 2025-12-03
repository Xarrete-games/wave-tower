extends MarginContainer

signal config_pressed()

func _on_config_config_pressed() -> void:
	config_pressed.emit()
