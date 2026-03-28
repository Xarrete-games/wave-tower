class_name PirateHat extends Relic

const CHANCE_TO_RECOVER_CONSUMABLE: float = 0.5

func on_consumable_used(consumable: Consumable) -> void:
	var value = randf()
	if value < CHANCE_TO_RECOVER_CONSUMABLE:
		RunContext.consumables_manager.add_consumable(consumable)