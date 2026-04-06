class_name LongShotBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var buff_duration = Duration.new(0, 1)
	return LongShotBuffScript.new(p_source, buff_duration, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_range_mult += 1
