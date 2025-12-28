class_name RunHandler extends Node

const NEXT_WAVE_SCREEN = preload("uid://b7ttkk4pasgin")
const NEXT_LEVEL_SCREEN = preload("uid://crastw7xnqgvl")
const END_GAME_SCENE = preload("uid://ovtc0l4cimpl")
const WAVES_WITH_EVENTS = [1,3,6,9]

@export var event_layer: CanvasLayer
@export var events_screen_hander: EventsScreenHandler
@export var rewards_screen_handler: RewardsScreenHandler 


func _ready() -> void:
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)
	RunContext.progress.last_wave_finished.connect(_on_last_wave_finished)
	RunContext.progress.current_level_changed.connect(_on_new_level_loaded)

# NEXT WAVE SCREEN
func _show_next_wave_screen() -> void:
	var next_wave_screen = NEXT_WAVE_SCREEN.instantiate()
	event_layer.call_deferred("add_child", next_wave_screen)

# NEXT LEVEL SCREEN
func _show_next_level_menu() -> void:
	var next_level_screen = NEXT_LEVEL_SCREEN.instantiate()
	event_layer.call_deferred("add_child", next_level_screen)

# REWARDS SCREEN
func _on_wave_finished() -> void:
	RunContext.economy.gold += 50
	if not GameState.is_on_main_menu():
		rewards_screen_handler.show_rewards_screen(event_layer)
		rewards_screen_handler.rewards_screen_close.connect(_on_rewards_screen_closed, CONNECT_ONE_SHOT)

func _on_last_wave_finished() -> void:
	RunContext.economy.gold += 50
	if RunContext.progress.is_last_level():
		await  get_tree().create_timer(5).timeout
		get_tree().change_scene_to_packed(END_GAME_SCENE)
	else:
		_show_next_level_menu()

func _on_rewards_screen_closed() -> void:
	if RunContext.progress.current_wave in WAVES_WITH_EVENTS:
		await events_screen_hander.show_events(event_layer)
	_show_next_wave_screen()

func _on_new_level_loaded(_level_num: int) -> void:
	_show_next_wave_screen()
