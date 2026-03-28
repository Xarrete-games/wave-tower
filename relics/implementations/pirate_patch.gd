class_name PiratePatch extends Relic

const HEALTH_AMOUNT: int = 5

func on_consumable_used(_consumable: Consumable) -> void:
	RunContext.status.heal(HEALTH_AMOUNT)