class_name EnemyWavesPanel extends Control

var wave_infos: Array[EnemyWaveInfo] = []
var level_waves: Array[EnemyWave] = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	level_waves = RunContext.level_data.enemy_waves
	print("[EnemyWavesPanel]: Loaded %d waves from level data" % [level_waves.size()])


