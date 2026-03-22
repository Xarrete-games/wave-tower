class_name PoisonPotion extends ConsumableUsable

func use() -> void:
	RunContext.status.apply_damage(10)