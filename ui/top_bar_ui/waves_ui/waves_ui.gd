class_name WavesUi extends Control

signal next_wave()

@export var enemy_generator: EnemyGenerator

@onready var next_wave_button: Button = $NextWaveButton

var _total_waves: int = 0
var _current_wave: int = 0

func _ready():
	enemy_generator.wave_finished.connect(_on_wave_finished)
	enemy_generator.wave_change.connect(_update_waves_counter)
	enemy_generator.new_level_loaded.connect(_on_new_level)
	
func _update_total_waves(new_value: int) -> void:
	_total_waves = new_value

func _update_waves_counter(new_value: int) -> void:
	_current_wave = new_value

func _on_wave_finished() -> void:
	_show_next_wave_button()

func _on_next_wave_button_pressed() -> void:
	next_wave.emit()
	visible = false
	next_wave_button.disabled = true
	
func _on_new_level() -> void:
	var total_waves = enemy_generator.total_waves
	
	_update_total_waves(total_waves)
	_show_next_wave_button()
	
func _show_next_wave_button() -> void:
	visible = true
	next_wave_button.disabled = false
	
	
	
