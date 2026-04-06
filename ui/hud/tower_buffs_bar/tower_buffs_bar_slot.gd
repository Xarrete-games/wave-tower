class_name TowerBuffsBarSlot
extends Control

@export var texture: TextureRect
@export var value_label: Label

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

func set_buff(tower_buff: TowerBuff, value: int = 0) -> void:
	if tower_buff == null:
		texture.texture = null
		value_label.text = ""
		return
	
	texture.texture = tower_buff.data.icon
	if value != 0:
		value_label.text = str(value)
	else:
		value_label.text = ""

func update_value(value: int) -> void:
	if value != 0:
		value_label.text = str(value)
	else:
		value_label.text = ""