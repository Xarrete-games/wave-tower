extends Control

signal config_pressed()


@export var pause : PackedScene

func _on_config_button_pressed() -> void:
	config_pressed.emit()

func pause_game() -> void:
	get_tree().paused = not get_tree().paused
	var pause_instance = pause.instantiate()
	add_child(pause_instance)
	#get_tree().change_scene_to_file("res://scenes/Volume_Control.tscn")
