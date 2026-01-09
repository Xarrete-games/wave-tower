class_name FlatDamageModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_damage += value
