class_name Hint extends Node

@export var text_label: Label
@export var title_label: Label
@export var container: Control


func set_text(text: String) -> void:
	text_label.text = text

func set_title(title: String) -> void:
	if title == "":
		title_label.visible = false
	else:
		title_label.visible = true
	title_label.text = title

func set_position(pos: Vector2) -> void:
	container.global_position = pos

func get_position() -> Vector2:
	return container.global_position

func get_size() -> Vector2:
	return container.size
