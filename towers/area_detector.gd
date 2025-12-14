class_name AreaDetector extends Area2D

signal target_change(enemy: Enemy)

var _targets_in_range: Array[Enemy] = []
var current_target: Enemy:
	set(value):
		current_target = value
		target_change.emit(value)

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
		#logic to select next target
		current_target = _targets_in_range[0]

func _on_enemy_die(enemy: Enemy) -> void:
	_remove_target_and_get_next(enemy)
