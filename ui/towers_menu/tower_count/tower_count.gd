@tool
class_name TowerCount extends Control

const TIER_1_COUNT = 2
const TIER_2_COUNT = 4
const TIER_3_COUNT = 6
const TIER_4_COUNT = 8

@export var icon: Texture2D:
	set(value):
		icon = value
		_update_texture()

var count: int = 0:
	set = _set_count
	
var max_value: int = TIER_1_COUNT:
	set(value):
		max_value = value
		max_label.text = str(max_value)

@onready var current_label: Label = $RedCount/Value/CurrentLabel
@onready var texture_rect: TextureRect = $RedCount/TextureRect
@onready var panel: Panel = $Panel

@onready var max_label: Label = $RedCount/Value/MaxLabel

func _ready() -> void:
	_update_texture()
	texture_rect.texture = icon

func _update_texture():
	if texture_rect:
		texture_rect.texture = icon

func _set_count(value: int) -> void:
	match value:
		var x when x < TIER_1_COUNT:
			max_value = TIER_1_COUNT
			pass
		var x when x >= TIER_1_COUNT and x < TIER_2_COUNT:
			max_value = TIER_2_COUNT
			pass
		var x when x >= TIER_2_COUNT and x < TIER_3_COUNT:
			max_value = TIER_3_COUNT
			pass
		var x when x >= TIER_3_COUNT and x < TIER_4_COUNT:
			max_value = TIER_4_COUNT
			pass
		var x when x > TIER_4_COUNT:
			pass
		
	count = value
	if count <= TIER_4_COUNT:
		current_label.text = str(count)
