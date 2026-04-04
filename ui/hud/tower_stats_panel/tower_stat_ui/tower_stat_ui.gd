@tool
class_name TowerStatUi extends Control


@export var stat_icon: Texture2D
@export var stat_value: float
@export var is_float: bool = false
@export var unit: String


@onready var stat_texture: TextureRect = %StatTexture
@onready var value_label: Label = %ValueLabel
@onready var unit_label: Label = %UnitLabel

func _ready() -> void:
	stat_texture.texture = stat_icon

	if unit == "":
		unit_label.visible = false
	else:
		unit_label.text = unit

	if is_float:
		value_label.text = str(snapped(stat_value, 0.01))
	else:
		value_label.text = str(int(stat_value))

func set_value(new_value: float) -> void:
	stat_value = new_value
	if is_float:
		value_label.text = str(stat_value)
	else:
		value_label.text = str(int(stat_value))