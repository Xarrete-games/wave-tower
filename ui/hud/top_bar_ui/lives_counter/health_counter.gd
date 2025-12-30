class_name HealthCounter extends Control

@onready var counter_label: Label = $CounterLabel

func _ready() -> void:
	RunContext.status.health_change.connect(func(value): health = value)
	health = RunContext.status.health

var health: int:
	set(value):
		health = value
		counter_label.text = str(value)
