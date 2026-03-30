class_name StrategyTomeLowHealthPriority extends Relic

func on_get_targeting_modes(targeting_modes: Array[Tower.TargetingMode]) -> void:
	targeting_modes.append(Tower.TargetingMode.LOW_HP)