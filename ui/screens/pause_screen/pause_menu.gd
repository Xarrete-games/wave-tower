class_name PauseMenu extends Control

var MAIN_MENU = load("uid://4i6kl0xurgeg")

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
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
