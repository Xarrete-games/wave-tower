class_name MainMenu extends Control

const BOOT = preload("uid://bfm0i7ehshgsf")
const CREDITS = preload("uid://bayb10jsajj4a")
const GAME_STATE_ON_MAIN_MENU := 0

@export var direct_init: bool = true

func _ready() -> void:
	AudioManager.play_main_piano()
	GameState.state = GAME_STATE_ON_MAIN_MENU
	if direct_init:
		_on_new_run_button_xarreta_pressed()

func _on_new_run_button_xarreta_pressed() -> void:
	AudioManager.stop_main_piano()
	call_deferred("_init_game")
	
func _on_credits_button_xarreta_pressed() -> void:
	get_tree().root.add_child(CREDITS.instantiate())

func _on_exit_button_xarreta_pressed() -> void:
	get_tree().quit()

func _init_game() -> void:
	get_tree().change_scene_to_packed(BOOT)
