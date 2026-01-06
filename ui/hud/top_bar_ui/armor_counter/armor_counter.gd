class_name ArmorCounter extends Control

var armor: int:
	set(value):
		armor = value
		counter_label.text = str(value)

@onready var counter_label: Label = $CounterLabel

func _ready() -> void:
	RunContext.status.armor_change.connect(func(value): armor = value)
	armor = RunContext.status.armor

