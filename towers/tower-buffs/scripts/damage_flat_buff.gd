class_name DamageFlatBuff extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source, p_value: int) -> TowerBuff:
	return DamageFlatBuff.new(p_source, null, null, p_data, p_value)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_damage += value
