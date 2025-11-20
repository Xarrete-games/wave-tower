class_name LivesCounter extends Control

@onready var counter_label: Label = $CounterLabel

func _ready() -> void:
	Score.lives_change.connect(func(value): lives = value)

var lives: int:
	set(value):
		lives = value
		counter_label.text = str(value)
