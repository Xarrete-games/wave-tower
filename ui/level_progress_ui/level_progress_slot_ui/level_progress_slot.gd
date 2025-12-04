class_name LevelProgressSlot extends PanelContainer

const LEVEL_PROGRESS_SLOT_FILL = preload("uid://ca11cx2n7wajo")
const PROGRESS_SLOT_EMPTY = preload("uid://d37mrhcqm5byv")

@export var texture_rec: TextureRect

func _ready() -> void:
	add_theme_stylebox_override("panel", PROGRESS_SLOT_EMPTY)

func set_icon(new_texture: Texture2D) -> void:
	texture_rec.texture = new_texture

func fill() -> void:
	add_theme_stylebox_override("panel", LEVEL_PROGRESS_SLOT_FILL)
