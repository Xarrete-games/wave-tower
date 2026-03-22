class_name AttackSpeedMultModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += value