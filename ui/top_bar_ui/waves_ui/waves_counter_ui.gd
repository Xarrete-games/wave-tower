class_name WavesCounterUi extends Control

@onready var waves_counter_current: Label = $HBoxContainer/WavesCounterCurrent
@onready var waves_counter_total: Label = $HBoxContainer/WavesCounterTotal
@onready var level_label: Label = $HBoxContainer2/LevelLabel

var _total_waves: int = 0
var _current_wave: int = 0

func _ready():
	EnemyManager.wave_init.connect(_update_waves_counter)
	EnemyManager.new_level_loaded.connect(_on_new_level)
	Settings.new_level_loaded.connect(_new_level_numer)

func _new_level_numer(level_num: int) -> void:
	if level_num == 3:
		level_label.text = "Final"
	else:
		level_label.text = str(level_num)

func _update_total_waves(new_value: int) -> void:
	waves_counter_total.text = str(new_value)
	_total_waves = new_value

func _update_waves_counter(new_value: int) -> void:
	waves_counter_current.text = str(new_value)
	_current_wave = new_value
	
func _on_new_level(total_waves: int) -> void:
	_update_total_waves(total_waves)
