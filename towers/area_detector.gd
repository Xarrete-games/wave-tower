class_name AreaDetector extends Area2D

signal target_change(enemy: Enemy)
signal enemy_die(enemy: Enemy)

var _targets_in_range: Array[Enemy] = []
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
	_select_next_target()

func _on_enemy_die(enemy: Enemy) -> void:
	enemy_die.emit(enemy)
	_remove_target_and_get_next(enemy)

func _select_next_target() -> void:
	if _targets_in_range.is_empty():
		current_target = null
		return
	match targeting_type:
		Tower.TargetingMode.FIRST_IN_PROGRESS:
			var enemy_with_highest_progress: Enemy = null
			var highest_progress: float = -1.0
			for enemy in _targets_in_range:
				if enemy.progress_ratio > highest_progress:
					highest_progress = enemy.progress_ratio
					enemy_with_highest_progress = enemy
			current_target = enemy_with_highest_progress
		Tower.TargetingMode.HIGHT_HP:
			if _targets_in_range.is_empty():
				current_target = null
				return
			var enemy_with_highest_hp: Enemy = null
			var highest_hp: float = -1.0
			for enemy in _targets_in_range:
				if enemy.get_remaining_heal() > highest_hp:
					highest_hp = enemy.get_remaining_heal()
					enemy_with_highest_hp = enemy
			current_target = enemy_with_highest_hp
		Tower.TargetingMode.LOW_HP:
			if _targets_in_range.is_empty():
				current_target = null
				return
			var enemy_with_lowest_hp: Enemy = null
			var lowest_hp: float = INF
			for enemy in _targets_in_range:
				if enemy.get_remaining_heal() < lowest_hp:
					lowest_hp = enemy.get_remaining_heal()
					enemy_with_lowest_hp = enemy
			current_target = enemy_with_lowest_hp