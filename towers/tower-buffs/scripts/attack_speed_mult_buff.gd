class_name AttackSpeedMultBuff extends TowerBuffStatsModifier

static func create_instance(p_data, p_source: Source, p_value: int) -> TowerBuff:
	return AttackSpeedMultBuff.new(p_source, null, null, p_data, p_value)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += float(value) / 100.0
