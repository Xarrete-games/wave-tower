class_name VicMicrophoneBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_RANGE_FROST, TowerStatsModifier.Mode.MULT, 0.2)
	return VicMicrophoneBuffScript.new(p_source, modifier, null, null, p_data)
