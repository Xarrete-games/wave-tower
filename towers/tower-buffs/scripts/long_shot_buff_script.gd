class_name LongShotBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_RANGE, TowerStatsModifier.Mode.MULT, 1)
	var buff_duration = Duration.new(0, 1)
	return LongShotBuffScript.new(p_source, modifier, buff_duration, null, p_data)
