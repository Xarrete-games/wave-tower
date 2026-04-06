class_name CaffeineBadBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var buff_duration = Duration.new(5, 0)
	return CaffeineBadBuffScript.new(p_source, buff_duration, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.attack_speed_mult += -0.2
