class_name StrategyTomeHightHealthPriority extends Relic

func apply_effect() -> void:
	var towers_upgrades = RunContext.towers_upgrades
	towers_upgrades.add_targeting_mode(Tower.TargetingMode.HIGHT_HP)
