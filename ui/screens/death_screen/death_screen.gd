class_name DeathScreen extends CanvasLayer

func _ready() -> void:
	AudioManager.play_defeated_sound()
	get_tree().paused = true

func _on_try_again_button_xarreta_pressed() -> void:
	get_tree().paused = false
	ClickEvents.emit_signal("reset_game_button_pressed")
	queue_free()
