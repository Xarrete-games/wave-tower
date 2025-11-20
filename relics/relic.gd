@abstract
class_name Relic extends RefCounted

enum RelicType { RED, BLUE, GREEN, COMMON }
enum RelicRarity { COMMON, RARE, EPIC}

var amount = 1
var id: String
var description: String
var type: RelicType
var rarity: RelicRarity
var max_stack: int
var texture: Texture2D

func _init() -> void:
	var data = get_data()
	id = data.id
	description = data.description
	type = data.type
	rarity = data.rarity
	max_stack = data.max_stack
	texture = data.texture

func get_id() -> String:
	return get_data().id

func get_description() -> String:
	return get_data().description

func get_texture() -> Texture2D:
	return get_data().texture

@abstract
func apply_effect() -> void

@abstract
func get_data() -> RelicData
