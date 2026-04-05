class_name PowerGlovesBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.DAMAGE, TowerStatsModifier.Mode.FLAT, 3)
	return PowerGlovesBuffScript.new(p_source, modifier, null, null, p_data)
