class_name PowerGlovesBuffScript extends TowerBuffStatsModifier

static func create_instance(p_data: BuffData, p_source: Source) -> TowerBuff:
	return PowerGlovesBuffScript.new(p_source, null, null, p_data)

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_damage += 3
