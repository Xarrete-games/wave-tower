class_name Game extends Node2D

const LEVELS_PATH = "res://levels/levels/"
const BOOT = preload("uid://bfm0i7ehshgsf")

@export var levels_paths: Array[String]
@export var pause : PackedScene

@export var trigger_finish_wave: bool = false

var _pause_instance: Control

#current level parent
@onready var level_container: Node2D = $LevelContainer
@onready var music_handler: MusicHandler = $MusicHandler
@onready var main_camera: MainCamera = $MainCamera
@onready var config_layer: CanvasLayer = $ConfigLayer

func _ready():
	ClickEvents.config_button_pressed.connect(_open_config_menu)
	ClickEvents.reset_game_button_pressed.connect(reset_game)
	RunContext.progress.total_levels = levels_paths.size()
	GameState.state = GameState.STATE.IN_GAME
	music_handler.play_music()
	if trigger_finish_wave:
		RunContext.progress.current_wave_finished.emit()

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
		if ActionManager.IsActionActive():
			ActionManager.EndAction()
		else:
			_open_config_menu()

func reset_game() -> void:
	RunContext.is_on_restarting = true
	var boot = load("uid://bfm0i7ehshgsf")
	get_tree().change_scene_to_packed(boot)
		
func _open_config_menu() -> void:
	if _pause_instance and _pause_instance.is_visible_in_tree():
		return
	get_tree().paused = not get_tree().paused
	_pause_instance = pause.instantiate()
	_pause_instance.resume_game.connect(_close_config_menu)
	config_layer.add_child(_pause_instance)

func _close_config_menu() -> void:
	if _pause_instance and _pause_instance.is_visible_in_tree():
		_pause_instance.queue_free()
		_pause_instance = null

		
	
	
