class_name PiratePatch extends Relic

const HEALTH_AMOUNT: int = 5

func apply_effect() -> void:
	RunContext.consumables_manager.consumable_used.connect(_on_consumable_used)

func remove_effect() -> void:
	RunContext.consumables_manager.consumable_used.disconnect(_on_consumable_used)

func _on_consumable_used(_consumable: Consumable) -> void:
	RunContext.status.heal(HEALTH_AMOUNT)