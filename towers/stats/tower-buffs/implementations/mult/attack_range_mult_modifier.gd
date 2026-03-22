class_name AttackRangeMultModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult += value