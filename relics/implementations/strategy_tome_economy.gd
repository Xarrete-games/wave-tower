class_name StrategyTomeEconomy extends Relic


func apply_effect() -> void:
	RunContext.economy.is_sell_active = true


func remove_effect() -> void:
	RunContext.economy.is_sell_active = false

