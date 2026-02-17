class_name EnemyDataLoader extends RefCounted

const DATA_PATH = "res://enemies/data/"

var enemies_data_dic: Dictionary[Enemy.TypeLegacy, EnemyData] = {
}

var enemies_data: Array[EnemyData] = []

func _init() -> void:
	var resources = _load_resources_from_dir(DATA_PATH)
	for data in resources:
		if not data is EnemyData:
			push_error("[EnemyDataLoader] Resource is not of type EnemyData: %s" % [data])
			return
		enemies_data.append(data as EnemyData)
	
	for enemy_data in enemies_data:
		enemies_data_dic[enemy_data.type_legacy] = enemy_data

func get_enemy_instance(enemy_type: Enemy.TypeLegacy) -> Enemy:

	var data = enemies_data_dic[enemy_type]
	var enemy_instance: Enemy = data.scene.instantiate() as Enemy
	enemy_instance.max_health = data.max_health
	enemy_instance.base_speed = data.base_speed
	enemy_instance.damage = data.damage
	enemy_instance.base_gold_value = data.base_gold_value

	return enemy_instance

func get_enemy_wave_info(enemy_type: Enemy.TypeLegacy) -> EnemyWaveInfo:
	var data = enemies_data_dic[enemy_type]
	var enemy_wave_info: EnemyWaveInfo = EnemyWaveInfo.new()
	enemy_wave_info.icon = data.icon
	enemy_wave_info.amount = 0
	enemy_wave_info.name = data.name
	enemy_wave_info.type = enemy_type
	return enemy_wave_info

func _load_resources_from_dir(path: String) -> Array[Resource]:
	var result: Array[Resource] = []

	var dir: DirAccess = DirAccess.open(path)
	if dir == null:
		push_error("[DataLoader] Directory not found: " + path)
		return result

	dir.list_dir_begin()
	var file: String = dir.get_next()

	while file != "":
		if file.ends_with(".tres"):
			var resource: Resource = load(path + file)
			if resource:
				result.append(resource)
		file = dir.get_next()

	dir.list_dir_end()

	return result
