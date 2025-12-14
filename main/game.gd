class_name Game extends Node2D

const LEVELS_PATH = "res://levels/levels/"
const GAME = preload("uid://6vgrx5dct8h8")

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
	ClickEvents.config_button_pressed.connect(_open_config_menu)
	ClickEvents.next_level_pressed.connect(go_next_level)
	ClickEvents.reset_game_button_pressed.connect(reset_game)
	GameState.reset_run()
	RunContext.reset_run()
	var items = RunContext.offers_manager.create_relic_offers(initial_random_relics)
	for item in items:
		var relic = item.create_item() as Relic
		RunContext.relics.add_relic(relic)
	RunContext.progress.total_levels = levels_paths.size()
	GameState.state = GameState.STATE.IN_GAME
	_load_level(current_level_number)

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
		if not GameState.is_placing_tower():
			_open_config_menu()

func reset_game() -> void:
	RunContext.is_on_restarting = true
	get_tree().change_scene_to_packed(GAME)

func _load_level(level_number: int) -> void:
	RunContext.progress.current_level = level_number
	music_handler.stop_music()
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
		
	
	
