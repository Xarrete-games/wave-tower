# LiveManager
extends Node

signal lives_change(amount: int)

const DEATH_SCENE = preload("uid://dcq16u6g6ahsp")
var death_fired: bool = false

var lives: int = 5:
	set(value):
		lives = value
		lives_change.emit(lives)
		if lives <= 0:
			if not death_fired:
				_on_die()
		else:
			death_fired = false
				
			
func _on_die() -> void:
	death_fired = true
	var death_scene = DEATH_SCENE.instantiate()
	get_tree().root.add_child(death_scene)
