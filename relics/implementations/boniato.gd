class_name Boniato extends Relic

func apply_effect() -> void:
	RunContext.economy.extra_gold_dropped += 1

func remove_effect() -> void:
	RunContext.economy.extra_gold_dropped -= 1
