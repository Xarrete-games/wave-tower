# DataLoader.gd
extends Node

const RELICS_DATA_PATH: String = "res://items/relics/data/"
const EVENTS_DATA_PATH: String = "res://events/data/"
const CONSUMABLES_DATA_PATH: String = "res://items/consumables/data/"

var relics: Array[RelicItemData] = []
var events: Array[EventData] = []
var consumables: Array[ConsumableItemData] = []
var enemy_data: EnemyDataLoader = EnemyDataLoader.new()

func _ready() -> void:
	_load_relics()
	_load_events()
	_load_consumables()

# ---------------------------------------------------------
# RELICS API
# ---------------------------------------------------------

func get_relic_by_id(relic_id: String) -> ItemData:
	for relic in relics:
		if relic.id == relic_id:
			return relic
	return null

func get_all_relics() -> Array[RelicItemData]:
	return relics.duplicate()

func get_not_used_relics() -> Array[RelicItemData]:
	return relics.filter(func(relic_data: RelicItemData):
		return not RunContext.relics_manager.is_maxed(relic_data.id)
	)
# ---------------------------------------------------------
# EVENTS API
# ---------------------------------------------------------

func get_all_events() -> Array[EventData]:
	return events.duplicate()

# ---------------------------------------------------------
# CONSUMABLES API
# ---------------------------------------------------------

func get_all_consumables() -> Array[ConsumableItemData]:
	return consumables.duplicate()

func get_all_consumables_of_type(consumable_type: Consumable.Type) -> Array[ConsumableItemData]:
	return consumables.filter(func(data: ConsumableItemData):
		return data.consumable_type == consumable_type
	)

# ---------------------------------------------------------
# INTERNAL LOADING HELPERS
# ---------------------------------------------------------
func _load_relics() -> void:
	var loaded_array = _load_resources_from_dir(RELICS_DATA_PATH)
	for data in loaded_array:
		if data is RelicItemData:
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
		if data is ConsumableItemData:
			consumables.append(data)
		else:
			push_error("[DataLoader] Loaded consumables data has invalid type: %s" % [data])


# ---------------------------------------------------------
# CORE GENERIC LOADER
# ---------------------------------------------------------
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
