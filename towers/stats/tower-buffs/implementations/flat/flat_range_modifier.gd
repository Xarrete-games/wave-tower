class_name FlatRangeModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
    acc.flat_attack_range += value