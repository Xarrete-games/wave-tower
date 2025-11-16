@abstract
class_name Relic extends RefCounted

enum RelicType { RED, BLUE, GREEN }

var amount = 1

func get_id() -> String:
	return get_data().id

func get_description() -> String:
	return get_data().description

func get_texture() -> Texture2D:
	return get_data().texture

@abstract
func apply_effect(tower_stats: TowerStats) -> void

@abstract
func get_data() -> RelicData
