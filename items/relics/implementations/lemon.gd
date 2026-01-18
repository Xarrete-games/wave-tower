class_name Lemon extends Relic

func apply_effect() -> void:
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)
	RunContext.towers_manager.tower_placed.connect(_on_tower_placed)

func remove_effect() -> void:
	RunContext.progress.current_wave_finished.disconnect(_on_wave_finished)
	RunContext.towers_manager.tower_placed.disconnect(_on_tower_placed)
	if RunContext.economy.is_lemon_active:
		RunContext.economy.towers_discount_mult += 0.5
		RunContext.economy.is_lemon_active = false

func _on_wave_finished() -> void:
	RunContext.economy.towers_discount_mult -= 0.5
	RunContext.economy.is_lemon_active = true

func _on_tower_placed(_tower_instance: Tower) -> void:
	if RunContext.economy.is_lemon_active:
		RunContext.economy.towers_discount_mult += 0.5
		RunContext.economy.is_lemon_active = false