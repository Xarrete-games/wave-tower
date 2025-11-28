class_name WavesUi extends Control

@onready var next_wave_button: Button = $NextWaveButton

func _ready():
	EnemyManager.wave_finished.connect(func (_wave: EnemyWave):
		visible = true
		next_wave_button.disabled = false
	)
	EnemyManager.new_level_loaded.connect(_show_next_wave_button)

func _on_next_wave_button_pressed() -> void:
	EnemyManager.next_wave_pressed.emit()
	visible = false
	next_wave_button.disabled = true
	
func _show_next_wave_button(_total_waves: int) -> void:
	visible = true
	next_wave_button.disabled = false
