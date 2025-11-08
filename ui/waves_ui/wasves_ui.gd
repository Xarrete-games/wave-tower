class_name WasvesUi extends Control

@export var enemy_generator: EnemyGenerator

@onready var next_wave_time: Label = $CenterContainer/NextWeveContainer/NextWaveTime
@onready var next_weve_container: VBoxContainer = $CenterContainer/NextWeveContainer
@onready var waves_counter_current: Label = $WavesCounterContainer/MarginContainer/HBoxContainer/WavesCounterCurrent
@onready var waves_counter_total: Label = $WavesCounterContainer/MarginContainer/HBoxContainer/WavesCounterTotal

var _current_seconds: int = 0
var _total_waves: int = 0
var _current_wave: int = 0

func _ready():
	pass
	
func _process(_delta: float) -> void:
	#time
	var next_wave_seconds_left = enemy_generator.get_next_wave_seconds_left()
	if _current_seconds != next_wave_seconds_left:
		_update_time(next_wave_seconds_left)
	
	#total wave:
	var total_waves = enemy_generator.total_waves
	if _total_waves != total_waves:
		_update_total_waves(total_waves)
		
	#total wave:
	var current_wave = enemy_generator.current_wave_number
	if _current_wave != current_wave:
		_update_current_waves(current_wave)
		
func _update_time(new_value: int) -> void:
	next_wave_time.text = str(new_value)
	_current_seconds = new_value
	next_weve_container.visible = _current_seconds != 0

func _update_total_waves(new_value: int) -> void:
	waves_counter_total.text = str(new_value)
	_total_waves = new_value

func _update_current_waves(new_value: int) -> void:
	waves_counter_current.text = str(new_value)
	_current_wave = new_value
