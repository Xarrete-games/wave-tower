class_name WavesCounterUi extends Control

signal next_wave()

@export var enemy_generator: EnemyGenerator

@onready var waves_counter_current: Label = $HBoxContainer/WavesCounterCurrent
@onready var waves_counter_total: Label = $HBoxContainer/WavesCounterTotal

var _total_waves: int = 0
var _current_wave: int = 0

func _ready():
	enemy_generator.wave_change.connect(_update_waves_counter)
	enemy_generator.new_level_loaded.connect(_on_new_level)
	
func _update_total_waves(new_value: int) -> void:
	waves_counter_total.text = str(new_value)
	_total_waves = new_value

func _update_waves_counter(new_value: int) -> void:
	waves_counter_current.text = str(new_value)
	_current_wave = new_value

func _on_next_wave_button_pressed() -> void:
	next_wave.emit()
	
func _on_new_level() -> void:
	var total_waves = enemy_generator.total_waves
	_update_total_waves(total_waves)
