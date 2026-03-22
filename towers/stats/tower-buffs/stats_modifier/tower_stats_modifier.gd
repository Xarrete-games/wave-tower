@abstract
class_name TowerStatsModifier extends RefCounted

var value: float = 0.0

func _init(p_value: float = 0) -> void:
	value = p_value

@abstract
func contribute(acc: TowerStatsAccumulator) -> void
