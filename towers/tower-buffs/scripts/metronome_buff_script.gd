class_name MetronomeBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_SPEED, TowerStatsModifier.Mode.MULT, -0.05)
	return TowerBuffStatsModifier.new(p_source, modifier, null, null, p_data)
