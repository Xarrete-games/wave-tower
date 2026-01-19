class_name EnemyWaveDataUi extends Control

const WAVE_DONE_COLOR: Color = Color.RED
const CURRENT_WAVE_COLOR: Color = Color.GREEN
const DEFAULT_COLOR: Color = Color.WHITE

@export var enemy_data_info_scene: PackedScene

var wave_number: int = 0
var info_list: Array[EnemyWaveInfo] = []
var current_wave: int = 0

@onready var label: RichTextLabel = $Label


func set_data(p_wave_number: int, p_info_list: Array[EnemyWaveInfo]) -> void:
	wave_number = p_wave_number
	info_list = p_info_list
	label.text = "[u]Wave %d[/u]" % p_wave_number
	# handle current wave color
	current_wave = RunContext.progress.current_wave

	_update_text_color()
	RunContext.progress.current_wave_changed.connect(func (new_wave: int) -> void:
		current_wave = new_wave
		_update_text_color()
	)
	
	for info in info_list:
		var enemy_data_ui: EnemyDataUi = enemy_data_info_scene.instantiate()
		add_child(enemy_data_ui)
		enemy_data_ui.set_data(info)

func _update_text_color() -> void:
	if wave_number == current_wave:
		label.add_theme_color_override("default_color", CURRENT_WAVE_COLOR)

	elif wave_number < current_wave:
		label.add_theme_color_override("default_color", WAVE_DONE_COLOR)
	else:
		label.add_theme_color_override("default_color", DEFAULT_COLOR)
