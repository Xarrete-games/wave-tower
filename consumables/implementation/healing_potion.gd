class_name HealingPotion extends ConsumableUsable

func  use() -> void:
	RunContext.status.heal(15)


