class_name GoldDropped extends Control

@export var time_to_vanish: float = 2

@onready var label: Label = $Label

func _ready():
	var tween1 = create_tween()
	
	tween1.tween_property(label, "position", Vector2(0, -10), time_to_vanish)
	var tween2 = create_tween()
	tween2.tween_property(label, "modulate:a", 0.0, time_to_vanish)
	await  tween2.finished
	queue_free()
	
func set_gold(new_value: int) -> void:
	label.text = "+" + str(new_value)
