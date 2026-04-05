class_name CaffeineBadBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	var modifier = TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_SPEED, TowerStatsModifier.Mode.MULT, -0.2)
	var buff_duration = Duration.new(5, 0)
	return CaffeineBadBuffScript.new(p_source, modifier, buff_duration, null, p_data)
