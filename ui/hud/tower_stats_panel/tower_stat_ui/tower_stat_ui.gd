class_name TowerStatUi extends Control

@export var stat_name: String
@export var stat_icon: Texture2D
@export var stat_value: float
@export var is_float: bool = false


@onready var stat_texture: TextureRect = %StatTexture
@onready var value_label: Label = %ValueLabel
@onready var attibute_label: Label = %AttributeLabel

func _ready() -> void:
	stat_texture.texture = stat_icon
	attibute_label.text = stat_name
	if is_float:
		value_label.text = str(stat_value)
	else:
		value_label.text = str(int(stat_value))

func set_value(new_value: float) -> void:
	stat_value = new_value
	if is_float:
		value_label.text = str(stat_value)
	else:
		value_label.text = str(int(stat_value))