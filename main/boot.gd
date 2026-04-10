class_name Boot extends Node

const GAME = preload("uid://6vgrx5dct8h8")
const PROCEDURAL_TEST = preload("uid://doun1k6w4e04m")

func _ready() -> void:
	GameState.reset_run()
	RunContext.reset_run()
	await get_tree().process_frame
	get_tree().change_scene_to_packed(PROCEDURAL_TEST)
