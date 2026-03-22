class_name PirateBlunderbuss extends Relic

func apply_effect() -> void:
	RunContext.loot_manager.chance_drop_consumable += 20

func remove_effect() -> void:
	RunContext.loot_manager.chance_drop_consumable -= 20