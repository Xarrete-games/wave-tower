class_name FlatRangeModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
    acc.flat_attack_range += value