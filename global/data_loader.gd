# DataLoader.gd
extends Node

const RELICS_DATA_PATH: String = "res://items/relics/data/"
const EVENTS_DATA_PATH: String = "res://events/data/"
const CONSUMABLES_DATA_PATH: String = "res://items/consumables/data/"

var relics: Array[ItemData] = []
var events: Array[EventData] = []
var consumables: Array[ItemData] = []
var enemy_data: EnemyDataLoader = EnemyDataLoader.new()

func _ready() -> void:
	_load_relics()
	_load_events()
	_load_consumables()

# ---------------------------------------------------------
# PUBLIC API
# ---------------------------------------------------------
func get_all_relics() -> Array[ItemData]:
	return relics.duplicate()

func get_all_events() -> Array[EventData]:
	return events.duplicate()

func get_all_consumables() -> Array[ItemData]:
	return consumables.duplicate()

# ---------------------------------------------------------
# INTERNAL LOADING HELPERS
# ---------------------------------------------------------
func _load_relics() -> void:
	var loaded_array = _load_resources_from_dir(RELICS_DATA_PATH)
	for data in loaded_array:
		if data is ItemData:
			relics.append(data)
		else:
			push_error("[DataLoader] Loaded relic has invalid type: %s" % [data])

func _load_events() -> void:
	var loaded_array = _load_resources_from_dir(EVENTS_DATA_PATH)
	for data in loaded_array:
		if data is EventData:
			events.append(data)
		else:
			push_error("[DataLoader] Loaded event data has invalid type: %s" % [data])

func _load_consumables() -> void:
	var loaded_array = _load_resources_from_dir(CONSUMABLES_DATA_PATH)
	for data in loaded_array:
		if data is ItemData:
			consumables.append(data)
		else:
			push_error("[DataLoader] Loaded consumables data has invalid type: %s" % [data])


# ---------------------------------------------------------
# CORE GENERIC LOADER
# ---------------------------------------------------------
func _load_resources_from_dir(path: String) -> Array[Resource]:
	var result: Array[Resource] = []

	var dir := DirAccess.open(path)
	if dir == null:
		push_error("[DataLoader] Directory not found: " + path)
		return result

	dir.list_dir_begin()
	var file := dir.get_next()

	while file != "":
		if file.ends_with(".tres"):
			var resource := load(path + file)
			if resource:
				result.append(resource)
		file = dir.get_next()

	dir.list_dir_end()

	#print("[DataLoader] Loaded ", result.size(), " items from: ", path)
	return result
