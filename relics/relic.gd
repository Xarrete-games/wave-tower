@abstract
class_name Relic extends RefCounted

var amount = 1

@abstract
func apply_effect(tower_stats: TowerStats) -> void

@abstract
func get_id() -> String

@abstract
func get_description() -> String

@abstract
func get_texture() -> Texture2D
