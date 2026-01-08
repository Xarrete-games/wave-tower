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
@onready var live_icon: TextureRect = $LiveIcon
@onready var armor_counter: ArmorCounter = $ArmorCounter


func _ready() -> void:
	RunContext.status.health_change.connect(func(value): health = value)
	RunContext.status.max_health_change.connect(func(value): max_health = value)
	RunContext.status.armor_change.connect(_on_armor_change)
	max_health = RunContext.status.max_health
	health = RunContext.status.health
	_on_armor_change(RunContext.status.armor)
	
func _on_armor_change(amount: int) -> void:
	if amount > 0:
		armor_counter.visible = true
		live_icon.visible = false
	else:
		armor_counter.visible = false
		live_icon.visible = true