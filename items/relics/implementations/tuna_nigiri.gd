class_name TunaNigiri extends Relic

func apply_effect() -> void:
	RunContext.economy.towers_discount_mult += 0.1

func remove_effect() -> void:
	RunContext.economy.towers_discount_mult -= 0.1