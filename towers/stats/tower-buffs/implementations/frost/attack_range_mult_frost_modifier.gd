class_name AttackRangeMultFrostModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult_frost += value

