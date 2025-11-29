class_name Level extends Node2D

@export var data: LevelData
@export var enemy_paths: EnemyPaths
@export var ememies_container: Node2D

@onready var camera_init_pos: Marker2D = $CameraInitPos

func _ready():
	Score.gold = data.initial_gold

func get_camera_init_pos() -> Vector2:
	return camera_init_pos.global_position

func get_waves() -> Array[EnemyWave]:
	if data == null:
		push_error("[Level]: waves data not assigned in %s" % [name])
	return data.enemy_waves

func get_enemy_paths() -> EnemyPaths:
	if enemy_paths == null:
		push_error("[Level]: enemy path not assigned in %s" % [name])
	return enemy_paths

func get_enemies_container() -> Node2D:
	if enemy_paths == null:
		push_error("[Level]: enemis_container not assigned in %s" % [name])
	return ememies_container
