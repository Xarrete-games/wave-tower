class_name Boot extends Node

const GAME = preload("uid://6vgrx5dct8h8")

func _ready() -> void:
	GameState.reset_run()
	RunContext.reset_run()
	get_tree().change_scene_to_packed(GAME)

