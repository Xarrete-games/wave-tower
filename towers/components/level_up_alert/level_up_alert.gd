class_name LevelUpAlert extends Control

@export var time_to_vanish: float = 3

@onready var level_up: Label = $LevelUp

func _ready():
	var tween1 = create_tween()
	var random_number: int = randi_range(-100, 100)
	tween1.tween_property(level_up, "position", Vector2(random_number, -30), time_to_vanish / 2)
	var tween2 = create_tween()
	tween2.tween_property(level_up, "modulate:a", 0.0, time_to_vanish / 2)
	await  tween2.finished
	queue_free()
