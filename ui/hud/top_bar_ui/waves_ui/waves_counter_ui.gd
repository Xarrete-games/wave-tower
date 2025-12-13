class_name WavesCounterUi extends Control

@onready var level_label: Label = $HBoxContainer2/LevelLabel

func _ready():
	await RunContext.run_reset
	RunContext.progress.current_level_changed.connect(_new_level_numer)

func _new_level_numer(level_num: int) -> void:
	level_label.text = str(level_num) + "/3"


	
