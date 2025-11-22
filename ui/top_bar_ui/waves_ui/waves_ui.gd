class_name WavesUi extends Control

@onready var next_wave_button: Button = $NextWaveButton

func _ready():
	EnemyGenerator.wave_finished.connect(_show_next_wave_button)
	EnemyGenerator.new_level_loaded.connect(_show_next_wave_button)

func _on_next_wave_button_pressed() -> void:
	EnemyGenerator.init_next_wave()
	visible = false
	next_wave_button.disabled = true
	
func _show_next_wave_button() -> void:
	visible = true
	next_wave_button.disabled = false
