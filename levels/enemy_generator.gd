class_name EnemyGenerator extends Node

signal group_handled()

var _level: Level
var level_waves: Array[EnemyWave]
var current_wave_number: int = 1
var total_waves: int
var current_wave: EnemyWave
var total_groups: int
var groups_handled_count: int
var groups_init_count: int
var _enemies_left: int

@onready var enemy_paths: EnemyPaths = $"../EnemyPaths"
@onready var visual: Node2D = $"../Visual"

func _ready() -> void:
	_level = get_parent()
	_load_level_data(_level)
	EnemyManager.next_wave_pressed.connect(init_next_wave)
	group_handled.connect(_on_group_handled)

func init_next_wave() -> void:
	# get next wave
	EnemyManager.wave_init.emit(current_wave_number)
	current_wave = level_waves[current_wave_number - 1]
	# reset groups counter
	total_groups = current_wave.groups.size()
	for group in current_wave.groups:
		# add extra group for double path
		var num_paths = group.paths.size()
		total_groups += (num_paths - 1)
			
	groups_handled_count = 0
	groups_init_count = 0
	# handle groups
	for enemy_group: EnemyGroup in current_wave.groups:
		if not enemy_group:
			push_error("[EnemyGenerator]: enemy group data is null")
			continue
		groups_init_count += 1
		_handle_ememy_group(
			enemy_group		
		)
	
	if groups_init_count == 0:
		await get_tree().create_timer(0.1).timeout
		push_error("[EnemyGenerator]: 0 enemy groups created force wave finish")
		_report_finished()
		
func _load_level_data(level: Level) -> void:
	_level = level
	level_waves = level.get_waves()
	total_waves = level_waves.size()
	current_wave_number = 1
	# inital wave data
	EnemyManager.new_level_loaded.emit(total_waves)
	EnemyManager.wave_init.emit(0)
	
func _handle_ememy_group(
	ememy_group: EnemyGroup) -> void:
	# wait time
	await get_tree().create_timer(ememy_group.time_to_start).timeout
	# individual group
	var paths = []

	for path in ememy_group.paths:
		_hand_enemy_group(
			ememy_group.enemy_type,
			ememy_group.amount,
			ememy_group.interval_spawn,
			path
		)
			
func _hand_enemy_group(
	enemy_type: Enemy.EnemyType,
	amount: int,
	interval_spawn: float,
	path: int
) -> void:
	for _i in range(amount):
		await get_tree().create_timer(interval_spawn).timeout
		_generate_enemy(enemy_type, path)
	group_handled.emit()
		
func _generate_enemy(enemy_type: Enemy.EnemyType, path: int) -> void:
	var enemy: Enemy = EnemyManager.get_enemy_scene(enemy_type).instantiate()
	enemy.die.connect(_on_enemy_die)
	enemy.target_reached.connect(_on_enemy_target_reached)
	visual.add_child(enemy)
	enemy.set_path_follow(enemy_paths.get_new_path_follow(path))
	enemy.enable()

func _on_group_handled() -> void:
	groups_handled_count += 1
	# all groups handled
	if groups_handled_count == total_groups:
		_enemies_left = get_tree().get_nodes_in_group("enemy").size()
		visual.child_exiting_tree.connect(_on_enemy_left)	

# When an enemy is eliminated (either by death or by reaching the end), 
# it is checked if there are more enemies left to finish the wave.
func _on_enemy_left(node: Node) -> void:
	if node.is_in_group("enemy"):
		_enemies_left -= 1
	if _enemies_left == 0:
		call_deferred("_check_enemies_left")

func _check_enemies_left() -> void:	
		_report_finished()
		if visual.child_exiting_tree.is_connected(_on_enemy_left):
			visual.child_exiting_tree.disconnect(_on_enemy_left)
		
# init the next wave or end the level if it's the last wave
func _report_finished() -> void:
	if LiveManager.lives <= 0:
		return
		
	if current_wave_number == total_waves:
		EnemyManager.last_wave_finished.emit(current_wave)
	else:
		current_wave_number += 1
		EnemyManager.wave_finished.emit(current_wave)
		
func _on_enemy_target_reached(enemy: Enemy) -> void:
	LiveManager.lives -= enemy.damage

func _on_enemy_die(enemy: Enemy) -> void:
	EnemyManager.enemy_die.emit(enemy)
	
