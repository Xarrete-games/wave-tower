class_name WasvesUi extends Control

@export var enemy_generator: EnemyGenerator

@onready var next_wave_time: Label = $CenterContainer/VBoxContainer/NextWaveTime

var _current_seconds = 0

func _ready():
	pass
	
func _process(delta: float) -> void:
	var next_wave_seconds_left = enemy_generator.get_next_wave_seconds_left()
	if _current_seconds != next_wave_seconds_left:
		_update_time(next_wave_seconds_left)

func _update_time(new_value: int) -> void:
	next_wave_time.text = str(new_value)
	_current_seconds = new_value
