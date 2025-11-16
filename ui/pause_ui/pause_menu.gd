extends Control

func _process(_delta: float) -> void:
	if Input.is_action_just_pressed("exit"):
		resume()

func resume():
	get_tree().paused = false
	queue_free()

func pause():
	get_tree().paused = true

func testEsc():
	if Input.is_action_just_pressed("esc") and get_tree().paused == false:
		pause()
	elif Input.is_action_just_pressed("esc") and get_tree().paused == true:
		resume()

func _on_resume_pressed() -> void:
	resume()

func _on_restart_pressed() -> void:
	resume()
	get_tree().reload_current_scene()
