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

## Returns all loaded enemy data resources.
func get_all_enemies() -> Array[EnemyData]:
	return enemies_data.duplicate()

## Returns enemies filtered by their pressure type (SWARM, FAST, NORMAL, TANK, BOSS).
func get_enemies_by_type(type: EnemyData.Type) -> Array[EnemyData]:
	return enemies_data.filter(func(data: EnemyData): return data.type == type)

## Returns all non-boss enemies (suitable for regular wave composition).
func get_spawnable_enemies() -> Array[EnemyData]:
	return enemies_data.filter(func(data: EnemyData): return data.type != EnemyData.Type.BOSS)

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
