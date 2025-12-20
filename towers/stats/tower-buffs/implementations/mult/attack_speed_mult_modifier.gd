class_name AttackSpeedMultModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += value