@abstract
class_name Relic extends RefCounted

var data: RelicData
var amount: int = 1
var disabled: bool = false

func _init(p_data: RelicData) -> void:
	data = p_data

@abstract
func apply_effect() -> void

@abstract
func remove_effect() -> void
