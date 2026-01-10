class_name AttackRangeMultFrostModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult_frost += value

