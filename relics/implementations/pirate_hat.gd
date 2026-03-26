class_name PirateHat extends Relic

const CHANCE_TO_RECOVER_CONSUMABLE: float = 0.5

func apply_effect() -> void:
	if not RunContext.consumables_manager.consumable_used.is_connected(_on_consumable_used):
		RunContext.consumables_manager.consumable_used.connect(_on_consumable_used)
	else:
		push_error("PirateHat: Already connected to consumable_used signal.") 

func remove_effect() -> void:
	if RunContext.consumables_manager.consumable_used.is_connected(_on_consumable_used):
		RunContext.consumables_manager.consumable_used.disconnect(_on_consumable_used)

func _on_consumable_used(consumable: Consumable) -> void:
	var value = randf()
	print(value)
	if value < CHANCE_TO_RECOVER_CONSUMABLE:
		RunContext.consumables_manager.add_consumable(consumable)