# LiveManager
extends Node

signal lives_change(amount: int)

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")

var lives: int = 5:
	set(value):
		lives = value
		lives_change.emit(lives)
		if lives <= 0:
			_on_die()
			
func _on_die() -> void:
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)
