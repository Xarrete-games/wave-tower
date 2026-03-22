class_name DamageMultModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.damage_mult += value
