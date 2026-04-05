class_name OverchargeAuraBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.DAMAGE, TowerStatsModifier.Mode.MULT, 0.1)
	return TowerBuffStatsModifier.new(p_source, modifier, null, null, p_data)
