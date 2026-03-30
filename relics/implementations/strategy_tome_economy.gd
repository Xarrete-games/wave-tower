class_name StrategyTomeEconomy extends Relic

func on_obtain() -> void:
	RunContext.economy.is_sell_active = true

func on_remove() -> void:
	RunContext.economy.is_sell_active = false