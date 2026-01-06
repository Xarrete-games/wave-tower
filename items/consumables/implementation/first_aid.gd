class_name FirstAid extends ConsumableUsable

func use() -> void:
	RunContext.status.health += 10

