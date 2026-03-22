class_name MagicRing extends ConsumableUsable

func use() -> void:
	RunContext.economy.available_free_towers += 1
