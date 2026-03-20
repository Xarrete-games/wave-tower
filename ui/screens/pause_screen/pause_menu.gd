class_name PauseMenu extends Control

var MAIN_MENU = load("uid://4i6kl0xurgeg")

@export var settings_section: Control
@export var menu_section: Control

func _ready() -> void:
	settings_section.visible = false
	menu_section.visible = true

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("exit"):
		if settings_section.visible:
			settings_section.visible = false
			menu_section.visible = true
		else:
			resume()

func resume():
	get_tree().paused = false
	queue_free()

func pause():
	get_tree().paused = true

func _on_resume_button_xarreta_pressed() -> void:
	resume()

func _on_restart_button_xarreta_pressed() -> void:
	resume()
	ClickEvents.reset_game_button_pressed.emit()

func _on_exit_button_xarreta_pressed() -> void:
	resume()
	GameState.state = GameState.STATE.ON_MAIN_MENU
	get_tree().change_scene_to_packed(MAIN_MENU)


func _on_back_button_xarreta_pressed() -> void:
	settings_section.visible = false
	menu_section.visible = true


func _on_settings_button_xarreta_pressed() -> void:
	print("Settings button pressed")
	settings_section.visible = true
	menu_section.visible = false
