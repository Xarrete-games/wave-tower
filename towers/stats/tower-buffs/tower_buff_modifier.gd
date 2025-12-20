@abstract
class_name TowerBuffModifier extends RefCounted

var value: float = 0.0

func _init(p_value: float) -> void:
	value = p_value

@abstract
func contribute(acc: TowerStatsAccumulator) -> void
