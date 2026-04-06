class_name OverchargeAuraBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	return OverchargeAuraBuffScript.new(p_source, null, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.damage_mult += 0.1
