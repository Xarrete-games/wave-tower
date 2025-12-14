class_name MagicRing extends Consumable

func use() -> void:
	RunContext.economy.available_free_towers += 1
