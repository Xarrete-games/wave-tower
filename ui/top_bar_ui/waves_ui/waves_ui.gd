class_name WavesUi extends Control

signal next_wave()

@export var enemy_generator: EnemyGenerator

@onready var next_wave_container: VBoxContainer = $CenterContainer/NextWaveContainer
@onready var waves_counter_current: Label = $WavesCounterContainer/MarginContainer/HBoxContainer/WavesCounterCurrent
@onready var waves_counter_total: Label = $WavesCounterContainer/MarginContainer/HBoxContainer/WavesCounterTotal
@onready var next_wave_button: Button = $CenterContainer/NextWaveContainer/NextWaveButton

var _total_waves: int = 0
var _current_wave: int = 0

func _ready():
	enemy_generator.wave_finished.connect(_on_wave_finished)
	enemy_generator.wave_change.connect(_update_waves_counter)
	enemy_generator.new_level_loaded.connect(_on_new_level)
	
func _update_total_waves(new_value: int) -> void:
	waves_counter_total.text = str(new_value)
	_total_waves = new_value

func _update_waves_counter(new_value: int) -> void:
	waves_counter_current.text = str(new_value)
	_current_wave = new_value

func _on_wave_finished() -> void:
	_show_next_wave_button()

func _on_next_wave_button_pressed() -> void:
	next_wave.emit()
	next_wave_container.visible = false
	next_wave_button.disabled = true
	
func _on_new_level() -> void:
	var total_waves = enemy_generator.total_waves
	
	_update_total_waves(total_waves)
	_show_next_wave_button()
	
func _show_next_wave_button() -> void:
	next_wave_container.visible = true
	next_wave_button.disabled = false
	
	
	
