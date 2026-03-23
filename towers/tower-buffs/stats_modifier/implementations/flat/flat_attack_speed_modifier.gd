class_name FlatAttackSpeedModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
    acc.flat_attack_speed += value