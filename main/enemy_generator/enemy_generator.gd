class_name EnemyGenerator extends Node

signal enemy_die(enemy: Enemy)

const ENEMY_NORMAL = preload("uid://dmqbn2q5splor")
const ENEMY_FAST = preload("uid://xk0wj86s8ddb")
const ENEMIES_SCENES: Dictionary = {
	Enemy.EnemyType.NORMAL: ENEMY_NORMAL,
	Enemy.EnemyType.FAST: ENEMY_FAST
}
var enemy_timer: Timer
var _level: Level
var waves_data: EnemyWavesData
var enemy_paths: EnemyPaths
var enemies_container: Node2D
var time_between_waves: int
var current_wave_number: int = 1
var current_wave: EnemyWave

@onready var next_wave_timer: Timer = $NextWaveTimer

#
func get_next_wave_seconds_left() -> int:
	return next_wave_timer.time_left
#call to load level_data
func load_level_nodes(level: Level) -> void:
	_level = level
	waves_data = level.get_waves_data()
	enemy_paths = level.get_enemy_paths()
	enemies_container = level.get_enemies_container()
	current_wave_number = 1
	
func init() -> void:
	next_wave_timer.wait_time = waves_data.time_between_waves
	next_wave_timer.start()
	next_wave_timer.timeout.connect(_on_next_wave_timer)
	
func _on_next_wave_timer() -> void:
	current_wave = waves_data.enemy_waves[current_wave_number - 1]
	
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
	#wait time
	
	await get_tree().create_timer(time_to_start).timeout
	
	if ememy_group is EnemyIndividualGroup:
		_hand_enemy_group(
			ememy_group.enemy_type,
			ememy_group.amount,
			ememy_group.interval_spawn,
			path
		)
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
		
func _generate_enemy(enemy_type: Enemy.EnemyType, path: int) -> void:
	var enemy: Enemy = ENEMIES_SCENES[enemy_type].instantiate()
	enemy.die.connect(_on_enemy_die)
	enemies_container.add_child(enemy)
	enemy.set_path_follow(enemy_paths.get_new_path_follow(path))
#func generate_enemies(level: Level) -> void:
	#_level = level
	#enemy_timer = Timer.new()
	#enemy_timer.wait_time = 3
	#enemy_timer.one_shot = false
	#enemy_timer.timeout.connect(_on_enemy_timer)
	#level.add_child(enemy_timer)
	#enemy_timer.start()
	#
#func _on_enemy_timer() -> void:
	## 1. Instanciar el enemigo
	#var new_enemy: Enemy = ENEMY.instantiate()
	#new_enemy.die.connect(_on_enemy_die)
	#get_tree().root.add_child(new_enemy)
	#
	#new_enemy.set_path_follow(_level.get_new_path_follow())
	#count += 1

func _on_enemy_die(enemy: Enemy) -> void:
	enemy_die.emit(enemy)
	
