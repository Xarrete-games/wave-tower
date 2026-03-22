class_name ExecuteThresholdModifier extends TowerStatsModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_execute_threshold += value