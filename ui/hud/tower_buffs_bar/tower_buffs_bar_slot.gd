class_name TowerBuffsBarSlot
extends Control

@export var texture: TextureRect
@export var value_label: Label

var tower_buff: TowerBuff
var value: int = 0:
	set(new_value):
		value = new_value
		if new_value != 0:
			value_label.text = str(new_value)
		else:
			value_label.text = ""

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

func set_buff(p_tower_buff: TowerBuff, p_value: int = 0) -> void:
	tower_buff = p_tower_buff
	value = p_value	
	texture.texture = tower_buff.data.icon
