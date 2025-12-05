class_name WavesCounterUi extends Control

@onready var level_label: Label = $HBoxContainer2/LevelLabel

func _ready():
	Settings.new_level_loaded.connect(_new_level_numer)

func _new_level_numer(level_num: int) -> void:
	level_label.text = str(level_num) + "/3"


	
