class_name AttackRangeMultModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult += value