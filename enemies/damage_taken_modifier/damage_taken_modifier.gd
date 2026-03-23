@abstract
class_name DamageTakenModifier extends RefCounted

var source: Source
var data: DamageTakenModifierData
var value: float

func _init(p_source: Source, p_value: float = 0.0) -> void:
	source = p_source
	value = p_value

@abstract
func modify_damage(enemy: Enemy, acc: DamageTakenModifierAcc) -> void
