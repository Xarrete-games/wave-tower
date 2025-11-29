class_name Game extends Node2D

@export var levels: Array[PackedScene]
@export var pause : PackedScene
@export var current_level_number: int = 1
@export var initial_random_relics: int = 0

var _current_level: Level

#current level parent
@onready var level_container: Node2D = $LevelContainer
@onready var tower_placer: TowerPlacer = $TowerPlacer
@onready var next_level_menu: NextLevelMenu = $UILayer/NextLevelMenu
@onready var music_handler: MusicHandler = $MusicHandler
@onready var ui_layer: CanvasLayer = $UILayer
@onready var main_camera: MainCamera = $MainCamera
@onready var config_layer: CanvasLayer = $ConfigLayer

func _ready():
	_hide_next_level_menu()
	_load_level(current_level_number)
	EnemyManager.last_wave_finished.connect(func(_wave: EnemyWave): _show_next_level_menu())
	await get_tree().create_timer(0.1).timeout
	RewardsManager.add_random_relics(initial_random_relics)

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
		if not TowerPlacementManager.is_placing:
			_open_config_menu()

func reset_current_level() -> void:
	if _current_level:
		_current_level.queue_free()
	RelicsManager.reset_relics()
	RewardsManager.reset_rewards()
	TowerPlacementManager.reset_towers()
	TowerUpgrades.reset_buffs()
	_load_level(initial_random_relics)

func _load_level(level_number: int) -> void:
	music_handler.stop_music()
	_current_level = levels[level_number - 1].instantiate()
	current_level_number = level_number
	level_container.add_child(_current_level)
	
	# on new level init
	_update_camera_post()
	tower_placer.update_nodes_from_current_level(_current_level)
	# RESET DATA
	TowerPlacementManager.reset_towers()
	music_handler.play_music()

func _hide_next_level_menu() -> void:
	next_level_menu.visible = false
	next_level_menu.mouse_filter = Control.MOUSE_FILTER_IGNORE
	next_level_menu.set_process(false)
	
func _show_next_level_menu() -> void:
	next_level_menu.visible = true
	next_level_menu.mouse_filter = Control.MOUSE_FILTER_STOP
	next_level_menu.set_process(true)

func _update_camera_post() -> void:
	var new_pos = _current_level.get_camera_init_pos()
	main_camera.global_position = new_pos

func _on_next_level_menu_next_leve_button_pressed() -> void:
	_hide_next_level_menu()
	current_level_number += 1
	_current_level.queue_free()
	if current_level_number  > levels.size():
		print("END GAME")
	else: 
		_load_level(current_level_number)
	
func _on_config_config_pressed() -> void:
	_open_config_menu()
	
func _open_config_menu() -> void:
	get_tree().paused = not get_tree().paused
	var pause_instance = pause.instantiate()
	config_layer.add_child(pause_instance)
