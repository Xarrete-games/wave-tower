@abstract
class_name Relic extends RefCounted

enum RelicType { RED, BLUE, GREEN, COMMON }
enum RelicRarity { COMMON, RARE, EPIC}

var amount = 1
var type: RelicType
var rarity: RelicRarity
var max_stack: int

func _init() -> void:
	var data = get_data()
	type = data.type
	rarity = data.rarity
	max_stack = data.max_stack

func get_id() -> String:
	return get_data().id

func get_description() -> String:
	return get_data().description

func get_texture() -> Texture2D:
	return get_data().texture

@abstract
func apply_effect(tower_buff: TowerBuff) -> void

@abstract
func get_data() -> RelicData
