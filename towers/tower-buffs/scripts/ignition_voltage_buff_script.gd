class_name IgnitionVoltageBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	return IgnitionVoltageBuffScript.new(p_source, null, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += 0.15
