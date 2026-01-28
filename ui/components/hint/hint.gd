class_name Hint extends Node

@export var label: Label
@export var container: Control

func set_text(text: String) -> void:
	label.text = text

func set_position(pos: Vector2) -> void:
	container.global_position = pos

func get_position() -> Vector2:
	return container.global_position

func get_size() -> Vector2:
	return container.size