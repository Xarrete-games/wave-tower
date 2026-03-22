class_name FlowerPot extends Relic

const HEALTH_BONUS: int = 1

var _enemies_reached_target: int = 0

func apply_effect() -> void:
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)
	RunContext.progress.current_wave_changed.connect(_on_wave_change)
	RunContext.enemy_manager.enemy_target_reached.connect(_on_enemy_target_reached)

func remove_effect() -> void:
	RunContext.progress.current_wave_finished.disconnect(_on_wave_finished)
	RunContext.progress.current_wave_changed.disconnect(_on_wave_change)
	RunContext.enemy_manager.enemy_target_reached.disconnect(_on_enemy_target_reached)

func _on_wave_change(_wave_num: int) -> void:
	_enemies_reached_target = 0

func _on_wave_finished() -> void:
	if _enemies_reached_target == 0:
		RunContext.status.health += HEALTH_BONUS

func _on_enemy_target_reached(_enemy: Enemy) -> void:
	_enemies_reached_target += 1