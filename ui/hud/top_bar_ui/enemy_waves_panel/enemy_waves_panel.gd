class_name EnemyWavesPanel extends Control

@export var enemy_wave_data_ui_scene: PackedScene
@export var info_container: Control

var wave_infos: Array[EnemyWaveInfo] = []
var level_waves: Array[EnemyWave] = []

func _ready() -> void:
	
	level_waves = RunContext.level_data.enemy_waves

	for wave_index in level_waves.size():
		var wave: EnemyWave = level_waves[wave_index]
		var enemy_wave_data_ui: EnemyWaveDataUi = enemy_wave_data_ui_scene.instantiate()
		info_container.add_child(enemy_wave_data_ui)
		
		var info_list: Array[EnemyWaveInfo] = []
		var used_enemy_types: Array[Enemy.Type] = []
		for enemy_group in wave.groups:
			if enemy_group.enemy_type in used_enemy_types:
				for info in info_list:
					if info.type == enemy_group.enemy_type:
						info.amount += enemy_group.amount
						break
				continue 
			used_enemy_types.append(enemy_group.enemy_type)
			var enemy_info: EnemyWaveInfo = DataLoader.enemy_data.get_enemy_wave_info(enemy_group.enemy_type)
			enemy_info.amount = enemy_group.amount

			info_list.append(enemy_info)
		
		enemy_wave_data_ui.set_data(wave_index + 1, info_list)