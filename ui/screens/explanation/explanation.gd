class_name Explanation extends Control

const MAIN_MENU = preload("uid://4i6kl0xurgeg")

func _on_defend_the_kingdom_pressed() -> void:
	get_tree().change_scene_to_packed(MAIN_MENU)
