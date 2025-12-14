class_name SalmonNigiri extends Relic

const discount = 10

func apply_effect() -> void:
	RunContext.economy.relics_discount_mult += 0.1


