class_name LivesCounter extends Control

@onready var couner_label: Label = $VBoxContainer/CounerLabel


func _ready() -> void:
	Score.lives_change.connect(func(value): lives = value)

var lives: int:
	set(value):
		lives = value
		couner_label.text = str(value)
