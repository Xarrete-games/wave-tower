class_name Game extends Node2D

signal new_level_loaded(level_num: int)
const END_GAME_SCENE = preload("uid://ovtc0l4cimpl")
const LEVELS_PATH = "res://levels/levels/"

@export var levels_paths: Array[String]
@export var pause : PackedScene
@export var current_level_number: int = 1
@export var initial_random_relics: int = 0

var _current_level: Level

#current level parent
@onready var level_container: Node2D = $LevelContainer
@onready var music_handler: MusicHandler = $MusicHandler
@onready var main_camera: MainCamera = $MainCamera
@onready var config_layer: CanvasLayer = $ConfigLayer

func _ready():
	ButtonsEvents.config_button_pressed.connect(_open_config_menu)
	GameState.reset_run()
	GameState.state = GameState.STATE.IN_GAME
	_load_level(current_level_number)
	
	await get_tree().create_timer(0.1).timeout
	RewardsManager.add_random_relics(initial_random_relics)

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
		if not TowerPlacementManager.is_placing:
			_open_config_menu()

func reset_current_level() -> void:
	if _current_level:
		_current_level.queue_free()
	GameState.reset_run()
	_load_level(1)

func _load_level(level_number: int) -> void:
	new_level_loaded.emit(level_number)
	music_handler.stop_music()
	Settings.new_level_loaded.emit(level_number)
	_current_level = _get_level(level_number)
	current_level_number = level_number
	level_container.add_child(_current_level)
	
	# on new level init
	_update_camera_post()
	# RESET DATA
	music_handler.play_music()

func _update_camera_post() -> void:
	var new_pos = _current_level.get_camera_init_pos()
	main_camera.global_position = new_pos

func go_next_level() -> void:
	current_level_number += 1
	_current_level.queue_free()
	if current_level_number  > levels_paths.size():
		push_error('[Game]: invalid go next level call')
	else: 
		_load_level(current_level_number)
		
func _open_config_menu() -> void:
	get_tree().paused = not get_tree().paused
	var pause_instance = pause.instantiate()
	config_layer.add_child(pause_instance)

func _get_level(level_number: int) -> Level:
	var path: String = LEVELS_PATH + str(level_number)
	var scene = SceneLoader.get_random_scene_from_path(path)
	return scene.instantiate()
		
	
	
