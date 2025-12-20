class_name DamageMultModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.damage_mult += value
