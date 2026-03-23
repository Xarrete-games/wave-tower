class_name FlatDamageModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_damage += value
