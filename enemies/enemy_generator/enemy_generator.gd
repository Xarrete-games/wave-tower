#EnemyGenerator 
extends Node

signal enemy_die(enemy: Enemy)

signal new_level_loaded()
signal last_wave_finished()
signal wave_finished()
signal wave_init(num: int)
# internal signal
signal group_handled()

const ENEMY_NORMAL = preload("uid://dmqbn2q5splor")
const ENEMY_FAST = preload("uid://xk0wj86s8ddb")
const ENEMY_TANK = preload("uid://dvri0e4k4qwho")
const ENEMY_GOLEM = preload("uid://cdb5n1d4ubx72")

const ENEMIES_SCENES: Dictionary = {
	Enemy.EnemyType.NORMAL: ENEMY_NORMAL,
	Enemy.EnemyType.FAST: ENEMY_FAST,
	Enemy.EnemyType.TANK: ENEMY_TANK,
	Enemy.EnemyType.GOLEM: ENEMY_GOLEM
}

@export var waves_ui: WavesUi

var enemy_timer: Timer
var _level: Level
var level_waves: Array[EnemyWave]
var enemy_paths: EnemyPaths
var enemies_container: Node2D
var time_between_waves: int
var current_wave_number: int = 1
var total_waves: int
var current_wave: EnemyWave
var total_groups: int
var groups_handled_count: int

func _ready() -> void:
	group_handled.connect(_on_group_handled)
#
# call to load level_data
func load_level_nodes(level: Level) -> void:
	_level = level
	level_waves = level.get_waves()
	enemy_paths = level.get_enemy_paths()
	enemies_container = level.get_enemies_container()
	total_waves = level_waves.size()
	current_wave_number = 1
	new_level_loaded.emit()
	wave_init.emit(0)
	
func init_next_wave() -> void:
	# get next wave
	wave_init.emit(current_wave_number)
	current_wave = level_waves[current_wave_number - 1]
	# reset groups counter
	total_groups = current_wave.groups_data.size()
	groups_handled_count = 0
	# handle groups
	for enemy_group_data: EnemyGroupData in current_wave.groups_data:
		_handle_ememy_group(
			enemy_group_data.enemy_group,
			enemy_group_data.time_to_start,
			enemy_group_data.path
		)

func _handle_ememy_group(
	ememy_group: EnemyGroup, 
	time_to_start: float, 
	path: int) -> void:
	# wait time
	await get_tree().create_timer(time_to_start).timeout
	# individual group
	if ememy_group is EnemyIndividualGroup:
		_hand_enemy_group(
			ememy_group.enemy_type,
			ememy_group.amount,
			ememy_group.interval_spawn,
			path
		)
	# multi group
	else :
		var multi_group = ememy_group as EnemyMultiGroups
		for next_group: EnemyIndividualGroup in multi_group.groups:
			_hand_enemy_group(
				next_group.enemy_type,
				next_group.amount,
				next_group.interval_spawn,
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
	var enemy: Enemy = ENEMIES_SCENES[enemy_type].instantiate()
	enemy.die.connect(_on_enemy_die)
	enemy.target_reached.connect(_on_enemy_target_reached)
	enemies_container.add_child(enemy)
	enemy.set_path_follow(enemy_paths.get_new_path_follow(path))

func _on_group_handled() -> void:
	groups_handled_count += 1
	if groups_handled_count == total_groups:
		enemies_container.child_exiting_tree.connect(_on_enemy_left)	

# When an enemy is eliminated (either by death or by reaching the end), 
# it is checked if there are more enemies left to finish the wave.
func _on_enemy_left(_node: Node) -> void:
	call_deferred("_check_enemies_left")

func _check_enemies_left() -> void:
	if enemies_container.get_child_count() == 0:
		_init_next_wave()
		if enemies_container.child_exiting_tree.is_connected(_on_enemy_left):
			enemies_container.child_exiting_tree.disconnect(_on_enemy_left)
		
# init the next wave or end the level if it's the last wave
func _init_next_wave() -> void:
	if current_wave_number == total_waves:
		print("LEVEL DONE")
		last_wave_finished.emit()
	else:
		current_wave_number += 1
		wave_finished.emit()
		
		#init_wave() 
func _on_enemy_target_reached(enemy: Enemy) -> void:
	LiveManager.lives -= enemy.damage

func _on_enemy_die(enemy: Enemy) -> void:
	enemy_die.emit(enemy)
	
