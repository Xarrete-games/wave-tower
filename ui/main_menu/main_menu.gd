class_name MainMenu extends Control

const GAME = preload("uid://6vgrx5dct8h8")
const CREDITS = preload("uid://bayb10jsajj4a")

@export var direct_init: bool = true

func _ready() -> void:
	if direct_init:
		_on_new_run_button_pressed()

func _on_exit_button_pressed() -> void:
	get_tree().quit()

func _on_credits_button_pressed() -> void:
	get_tree().root.add_child(CREDITS.instantiate())

func _on_new_run_button_pressed() -> void:
	get_tree().change_scene_to_packed(GAME)
