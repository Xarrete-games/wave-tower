# DataLoader.gd
extends Node

const RELICS_DATA_PATH: String = "res://items/relics/data/"
const EVENTS_DATA_PATH: String = "res://events/data/"
const CONSUMABLES_DATA_PATH: String = "res://items/consumables/data/"
const INITIAL_MAP_PIECES_DATA_PATH: String = "res://levels/map_pieces/init/"
const MAP_PIECES_DATA_PATH: String = "res://levels/map_pieces/data/"
const TOWER_DATA_PATH: String = "res://towers/data/"

var relics: Array[RelicData] = []
var events: Array[EventData] = []
var consumables: Array[ConsumableData] = []
var initial_map_pieces: Array[MapPieceData] = []
var map_pieces: Array[MapPieceData] = []
var tower_data: Array[TowerDataWithInstance] = []
var enemy_data: EnemyDataLoader = EnemyDataLoader.new()

func _ready() -> void:
	_load_relics()
	_load_events()
	_load_consumables()
	_load_map_pieces()
	_load__initial_map_pieces()
	_load_tower_data()

# ---------------------------------------------------------
# RELICS API
# ---------------------------------------------------------

func get_relic_by_id(relic_id: String) -> RelicData:
	for relic in relics:
		if relic.id == relic_id:
			return relic
	return null

func get_all_relics() -> Array[RelicData]:
	return relics.duplicate()

func get_not_used_relics(rarity: Relic.Rarity = Relic.Rarity.ALL, is_cursed: Variant = null) -> Array[RelicData]:
	return relics.filter(func(relic_data: RelicData):

		# If we want a specific rarity
		if rarity != Relic.Rarity.ALL and relic_data.rarity != rarity:
			return false

		# If we want only non-cursed relics
		if is_cursed != null: 
			if not is_cursed and relic_data.is_cursed:
				return false
			elif is_cursed and not relic_data.is_cursed:
				return false

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

func get_consumable_by_id(consumable_id: String) -> ConsumableData:
	for consumable in consumables:
		if consumable.id == consumable_id:
			return consumable
	return null

func get_all_consumables() -> Array[ConsumableData]:
	return consumables.duplicate()

func get_all_consumables_of_type(consumable_type: Consumable.Type) -> Array[ConsumableData]:
	return consumables.filter(func(data: ConsumableData):
		return data.consumable_type == consumable_type
	)

# ---------------------------------------------------------
# MAP PIECES API
# ---------------------------------------------------------

func get_all_initial_map_pieces() -> Array[MapPieceData]:
	return initial_map_pieces.duplicate()

func get_all_map_pieces() -> Array[MapPieceData]:
	return map_pieces.duplicate()

# ---------------------------------------------------------
# ENEMIES API
# ---------------------------------------------------------

## Returns all loaded enemy data resources.
func get_all_enemies() -> Array[EnemyData]:
	return enemy_data.get_all_enemies()

## Returns enemies filtered by their pressure type.
func get_enemies_by_type(type: EnemyData.Type) -> Array[EnemyData]:
	return enemy_data.get_enemies_by_type(type)

## Returns all non-boss enemies (for regular wave composition).
func get_spawnable_enemies() -> Array[EnemyData]:
	return enemy_data.get_spawnable_enemies()

# ---------------------------------------------------------
# TOWERS API
# ---------------------------------------------------------

func get_all_tower_data() -> Array[TowerDataWithInstance]:
	return tower_data.duplicate()
	
# ---------------------------------------------------------
# INTERNAL LOADING HELPERS
# ---------------------------------------------------------
func _load_relics() -> void:
	var loaded_array = _load_resources_from_dir(RELICS_DATA_PATH)
	for data in loaded_array:
		if data is RelicData:
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
		if data is ConsumableData:
			consumables.append(data)
		else:
			push_error("[DataLoader] Loaded consumables data has invalid type: %s" % [data])

func _load_map_pieces() -> void:
	var loaded_array = _load_resources_from_dir(MAP_PIECES_DATA_PATH)
	for data in loaded_array:
		if data is MapPieceData:
			map_pieces.append(data)
		else:
			push_error("[DataLoader] Loaded map piece data has invalid type: %s" % [data])

func _load__initial_map_pieces() -> void:
	var loaded_array = _load_resources_from_dir(INITIAL_MAP_PIECES_DATA_PATH)
	for data in loaded_array:
		if data is MapPieceData:
			initial_map_pieces.append(data)
		else:
			push_error("[DataLoader] Loaded initial map piece data has invalid type: %s" % [data])

func _load_tower_data() -> void:
	var loaded_array = _load_resources_from_dir(TOWER_DATA_PATH)
	for data in loaded_array:
		if data is TowerDataWithInstance:
			tower_data.append(data)
		else:
			push_error("[DataLoader] Loaded tower data has invalid type: %s" % [data])


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
