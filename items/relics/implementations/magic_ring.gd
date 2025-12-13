class_name MagicRing extends Relic

func apply_effect() -> void:
	RunContext.economy.available_free_towers += 1
