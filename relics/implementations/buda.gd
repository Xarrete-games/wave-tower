class_name Buda extends Relic

func on_tower_placed(_tower: Tower) -> void:
	RunContext.status.max_health += 1