class_name HealthCounter extends Control

var health: int:
	set(value):
		health = value
		counter_label.text = str(value)

var max_health: int:
	set(value):
		max_health = value
		counter_max_label.text = str(value)

@onready var counter_max_label: Label = $CounterMaxLabel
@onready var counter_label: Label = $CounterLabel

func _ready() -> void:
	RunContext.status.health_change.connect(func(value): health = value)
	RunContext.status.max_health_change.connect(func(value): max_health = value)
	max_health = RunContext.status.max_health
	health = RunContext.status.health


