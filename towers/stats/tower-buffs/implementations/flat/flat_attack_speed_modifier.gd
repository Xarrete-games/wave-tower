class_name FlatAttackSpeedModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
    acc.flat_attack_speed += value