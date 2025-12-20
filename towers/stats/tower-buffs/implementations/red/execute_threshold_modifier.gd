class_name ExecuteThresholdModifier extends TowerBuffModifier

func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_execute_threshold += value