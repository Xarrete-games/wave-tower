class_name DeathScreen extends CanvasLayer

func _ready() -> void:
	get_tree().paused = true

func _on_try_again_button_xarreta_pressed() -> void:
	get_tree().paused = false
	await get_tree().create_timer(0.1).timeout
	var game: Game = get_tree().root.get_node("Game")
	game.reset_current_level()
	queue_free()
