class_name StrategyTomeLowHealthPriority extends Relic

func apply_effect() -> void:
	var towers_upgrades = RunContext.towers_buffs
	towers_upgrades.add_targeting_mode(Tower.TargetingMode.LOW_HP)

func remove_effect() -> void:
	var towers_upgrades = RunContext.towers_buffs
	towers_upgrades.remove_targeting_mode(Tower.TargetingMode.LOW_HP)