class_name WavesCounterUi extends Control

@onready var level_label: Label = $HBoxContainer2/LevelLabel

func _ready():
	RunContext.progress.current_wave_changed.connect(_on_wave_change)

func _on_wave_change(wave_num: int) -> void:
	level_label.text = str(wave_num)


	
