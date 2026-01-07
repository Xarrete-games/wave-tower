class_name Buda extends Relic


func apply_effect() -> void:
	RunContext.towers_count.tower_placed.connect(_on_tower_placed)

func _on_tower_placed(_tower: Tower) -> void:
	RunContext.status.max_health += 1