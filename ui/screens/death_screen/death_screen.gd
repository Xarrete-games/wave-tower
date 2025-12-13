class_name DeathScreen extends CanvasLayer

func _ready() -> void:
	get_tree().paused = true

func _on_try_again_button_xarreta_pressed() -> void:
	get_tree().paused = false
	ButtonsEvents.reset_game_button_pressed.emit()
	queue_free()
