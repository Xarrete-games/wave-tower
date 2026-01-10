class_name StrategyTomeHightHealthPriority extends Relic

func apply_effect() -> void:
	RunContext.towers_buffs.add_targeting_mode(Tower.TargetingMode.HIGH_HP)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_targeting_mode(Tower.TargetingMode.HIGH_HP)