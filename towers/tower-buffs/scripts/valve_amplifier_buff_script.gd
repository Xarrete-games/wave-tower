class_name ValveAmplifierBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	return ValveAmplifierBuffScript.new(p_source, null, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult += 0.1
