class_name MetronomeBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	return MetronomeBuffScript.new(p_source, null, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += -0.05
