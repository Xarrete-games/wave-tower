class_name AttackSpeedMultLightningModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult_lightning += value