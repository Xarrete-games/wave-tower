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

func _on_resume_pressed() -> void:
	resume()

func _on_restart_pressed() -> void:
	resume()
	get_tree().root.get_node("Game").reset_current_level()

func _on_exit_button_pressed() -> void:
	resume()
	GameState.reset_run()
	get_tree().change_scene_to_packed(MAIN_MENU)
