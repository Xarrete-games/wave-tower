class_name DamageMultBuff extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source, p_value: float) -> TowerBuff:
	return DamageMultBuff.new(p_source, null, null, p_data, p_value)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.damage_mult += value
