class_name EnemyWaveDataUi extends Control

@export var enemy_data_info_scene: PackedScene

var wave_number: int = 0
var info_list: Array[EnemyWaveInfo] = []

@onready var label: RichTextLabel = $Label

func set_data(p_wave_number: int, p_info_list: Array[EnemyWaveInfo]) -> void:
	wave_number = p_wave_number
	info_list = p_info_list
	label.text = "[u]Wave %d[/u]" % p_wave_number
	
	for info in info_list:
		var enemy_data_ui: EnemyDataUi = enemy_data_info_scene.instantiate()
		add_child(enemy_data_ui)
		enemy_data_ui.set_data(info)