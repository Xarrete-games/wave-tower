class_name PirateHat extends Relic

const CHANCE_TO_RECOVER_CONSUMABLE: float = 0.5

func apply_effect() -> void:
	RunContext.consumables_manager.consumable_used.connect(_on_consumable_used)

func remove_effect() -> void:
	RunContext.consumables_manager.consumable_used.disconnect(_on_consumable_used)

func _on_consumable_used(consumable: Consumable) -> void:
	if randi() % 100 < CHANCE_TO_RECOVER_CONSUMABLE * 100:
		RunContext.consumables_manager.add_consumable(consumable)