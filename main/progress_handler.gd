class_name ProgressHandler extends Node

const NEXT_WAVE_SCREEN = preload("uid://b7ttkk4pasgin")
const NEXT_LEVEL_SCREEN = preload("uid://crastw7xnqgvl")
const END_GAME_SCENE = preload("uid://ovtc0l4cimpl")

@export var event_layer: CanvasLayer

var _current_level: int = 0
var _total_levels: int = 0
var game: Game

func _ready() -> void:
	EnemyManager.wave_finished.connect(_on_wave_finished)
	EnemyManager.last_wave_finished.connect(func(_wave: EnemyWave): _show_next_level_menu())
	# TODO do it better
	await get_tree().create_timer(0.1).timeout
	game = get_parent()
	_total_levels = game.levels_paths.size()

# NEXT WAVE SCREEN
func _show_next_wave_screen() -> void:
	var next_wave_screen = NEXT_WAVE_SCREEN.instantiate()
	event_layer.add_child(next_wave_screen)
	next_wave_screen.tree_exited.connect(func(): EnemyManager.next_wave_pressed.emit(), CONNECT_ONE_SHOT)

# NEXT LEVEL SCREEN
func _show_next_level_menu() -> void:
	var next_level_screen = NEXT_LEVEL_SCREEN.instantiate()
	event_layer.add_child(next_level_screen)
	next_level_screen.tree_exited.connect(func(): game.go_next_level(), CONNECT_ONE_SHOT)

# REWARDS SCREEN
func _on_wave_finished(_wave: EnemyWave) -> void:
	if not GameState.is_on_main_menu():
		RewardsManager.show_rewards_ui(event_layer)
		RewardsManager.rewards_ui_closed.connect(_on_rewards_ui_closed, CONNECT_ONE_SHOT)

func _on_last_wave_finished(_wave: EnemyWave) -> void:
	if _current_level == _total_levels:
		await  get_tree().create_timer(5).timeout
		get_tree().change_scene_to_packed(END_GAME_SCENE)
	else:
		_show_next_level_menu()

func _on_rewards_ui_closed() -> void:
	_show_next_wave_screen()

func _on_game_new_level_loaded(_level_num: int) -> void:
	_show_next_wave_screen()
