class_name AreaDetector extends Area2D

signal target_change(enemy: Enemy)

var _targets_in_range: Array[Enemy] = []
var current_target: Enemy:
	set(value):
		current_target = value
		target_change.emit(value)

var targeting_type: Tower.TargetingMode = Tower.TargetingMode.FIRST_IN_PROGRESS

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	enemy.die.connect(_on_enemy_die)
	
	_targets_in_range.append(enemy)

	if current_target == null:
		current_target = enemy
	
func _on_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	_remove_target_and_get_next(enemy)
	
func _remove_target_and_get_next(enemy: Enemy) -> void:
	# disconnect signal
	if enemy.die.is_connected(_on_enemy_die):
		enemy.die.disconnect(_on_enemy_die)
	# remove enemy
	_targets_in_range.erase(enemy)
	#exit when enemy is not the target
	if enemy != current_target:
		return
	if _targets_in_range.is_empty():
		current_target = null
	else:
		_select_next_target()

func _on_enemy_die(enemy: Enemy) -> void:
	_remove_target_and_get_next(enemy)

func _select_next_target() -> void:
	if targeting_type == Tower.TargetingMode.FIRST_IN_PROGRESS:
		var enemy_with_highest_progress: Enemy = null
		var highest_progress: float = -1.0
		for enemy in _targets_in_range:
			if enemy.progress_ratio > highest_progress:
				highest_progress = enemy.progress_ratio
				enemy_with_highest_progress = enemy
		current_target = enemy_with_highest_progress
	