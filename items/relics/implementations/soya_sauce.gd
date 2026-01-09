class_name SoyaSauce extends Relic

func apply_effect() -> void:
	RunContext.economy.is_soya_sauce_active = true

func remove_effect() -> void:
	RunContext.economy.is_soya_sauce_active = false



