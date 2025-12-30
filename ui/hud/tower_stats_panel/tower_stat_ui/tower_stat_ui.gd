class_name TowerStatUi extends Control

@export var stat_name: String
@export var stat_icon: Texture2D
@export var stat_value: float


@onready var stat_texture: TextureRect = %StatTexture
@onready var value_label: Label = %ValueLabel
@onready var attibute_label: Label = %AttributeLabel

func _ready() -> void:
	stat_texture.texture = stat_icon
	attibute_label.text = stat_name
	value_label.text = str(int(stat_value))

func set_value(new_value: float) -> void:
	stat_value = new_value
	value_label.text = str(int(stat_value))