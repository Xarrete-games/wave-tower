class_name AreaDetector extends Area2D

signal target_change(enemy: Enemy)
signal enemy_die(enemy: Enemy)

var targets_in_range: Array[Enemy] = []
var current_target: Enemy:
	set(value):
		current_target = value
		target_change.emit(value)

var targeting_type: Tower.TargetingMode = Tower.TargetingMode.FIRST_IN_PROGRESS

@onready var timer_to_check_target: Timer = $TimerToCheckTarget

func _ready() -> void:
	timer_to_check_target.timeout.connect(_select_next_target)

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	if enemy == null or not enemy.enabled:
		return
	enemy.tree_exited.connect(func() -> void:
		_on_enemy_die(enemy))
	targets_in_range.append(enemy)

	if current_target == null:
		current_target = enemy
	
func _on_body_exited(body: Node2D) -> void:
	var enemy = body as Enemy
	_remove_target_and_get_next(enemy)
	
func _remove_target_and_get_next(enemy: Enemy) -> void:
	_prune_invalid_targets()
	# remove enemy
	targets_in_range.erase(enemy)
	#exit when enemy is not the target
	if enemy != current_target:
		return
	_select_next_target()

func _on_enemy_die(enemy: Enemy) -> void:
	enemy_die.emit(enemy)
	_remove_target_and_get_next(enemy)


func _prune_invalid_targets() -> void:
	for i in range(targets_in_range.size() - 1, -1, -1):
		var enemy: Enemy = targets_in_range[i]
		if enemy == null or not is_instance_valid(enemy):
			targets_in_range.remove_at(i)

	if current_target != null and not is_instance_valid(current_target):
		current_target = null

func _select_next_target() -> void:
	_prune_invalid_targets()
	if targets_in_range.is_empty() or not monitoring:
		current_target = null
		return
	match targeting_type:
		Tower.TargetingMode.FIRST_IN_PROGRESS:
			var enemy_with_highest_progress: Enemy = null
			var highest_progress: float = -1.0
			for enemy in targets_in_range:
				if enemy == null or not is_instance_valid(enemy):
					continue
				var ratio: float = enemy.get_progress_ratio()
				if ratio > highest_progress:
					highest_progress = ratio
					enemy_with_highest_progress = enemy
			current_target = enemy_with_highest_progress
		Tower.TargetingMode.HIGH_HP:
			if targets_in_range.is_empty():
				current_target = null
				return
			var enemy_with_highest_hp: Enemy = null
			var highest_hp: float = -1.0
			for enemy in targets_in_range:
				if enemy == null or not is_instance_valid(enemy):
					continue
				if enemy.get_remaining_health() > highest_hp:
					highest_hp = enemy.get_remaining_health()
					enemy_with_highest_hp = enemy
			current_target = enemy_with_highest_hp
		Tower.TargetingMode.LOW_HP:
			if targets_in_range.is_empty():
				current_target = null
				return
			var enemy_with_lowest_hp: Enemy = null
			var lowest_hp: float = INF
			for enemy in targets_in_range:
				if enemy == null or not is_instance_valid(enemy):
					continue
				if enemy.get_remainig_health() < lowest_hp:
					lowest_hp = enemy.get_remainig_health()
					enemy_with_lowest_hp = enemy
			current_target = enemy_with_lowest_hp

func clear_targets() -> void:
	targets_in_range.clear()
	current_target = null
