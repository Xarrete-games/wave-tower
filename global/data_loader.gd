# DataLoader.gd
extends Node

const RELICS_DATA_PATH: String = "res://relics/data/"
const EVENTS_DATA_PATH: String = "res://events/data/"
const CONSUMABLES_DATA_PATH: String = "res://consumables/data/"
const ENEMY_DEBUFFS_DATA_PATH: String = "res://enemies/enemy_debuff/data/"
const INITIAL_MAP_PIECES_DATA_PATH: String = "res://levels/map_pieces/init/"
const MAP_PIECES_DATA_PATH: String = "res://levels/map_pieces/data/"
const TOWER_DATA_PATH: String = "res://towers/data/"

var relics: Array[RelicData] = []
var events: Array[EventData] = []
var consumables: Array[ConsumableData] = []
var enemy_debuffs: Array[EnemyDebuffData] = []
var initial_map_pieces: Array[MapPieceData] = []
var map_pieces: Array[MapPieceData] = []
var tower_data: Array[TowerDataWithInstance] = []
var enemy_data: EnemyDataLoader = EnemyDataLoader.new()

func _ready() -> void:
	_load_relics()
	_load_events()
	_load_consumables()
	_load_enemy_debuffs()
	_load_map_pieces()
	_load__initial_map_pieces()
	_load_tower_data() 

# ---------------------------------------------------------
# RELICS API
# ---------------------------------------------------------

func get_relic_by_id(relic_id: String) -> RelicData:
	for relic in relics:
		if relic.id == relic_id:
			return _duplicate_resource(relic) as RelicData
	return null

func get_all_relics() -> Array[RelicData]:
	var result: Array[RelicData] = []
	_append_deep_copies(relics, result)
	return result

func get_random_relics(amount: int, rarity: Variant = null, include_cursed: bool = false, include_only_for_events: bool = false) -> Array[RelicData]:
	var candidates = get_not_used_relics(rarity)
	candidates = candidates.filter(func(relic_data: RelicData):
		if not include_cursed and relic_data.is_cursed:
			return false
		if not include_only_for_events and relic_data.only_for_events:
			return false
		return true
	)

	candidates.shuffle()

	var result: Array[RelicData] = []
	for relic_data in candidates:
		if result.size() >= amount:
			break
		result.append(relic_data)

	return result

func get_not_used_relics(rarity: Variant = null, is_cursed: Variant = null) -> Array[RelicData]:
	var filtered: Array[RelicData] = relics.filter(func(relic_data: RelicData):

		# If we want a specific rarity
		if rarity != null and relic_data.rarity != rarity:
			return false

		# If we want only non-cursed relics
		if is_cursed != null: 
			if not is_cursed and relic_data.is_cursed:
				return false
			elif is_cursed and not relic_data.is_cursed:
				return false

		return not RunContext.relics_manager.has_relic(relic_data.id)
	)

	var result: Array[RelicData] = []
	_append_deep_copies(filtered, result)
	return result

func get_random_available_relics(
	amount: int,
	rarity: Variant = null,
	include_cursed: bool = false,
	include_only_for_events: bool = false
) -> Array[RelicData]:
	var candidates = get_not_used_relics(rarity)
	candidates = candidates.filter(func(relic_data: RelicData):
		if not include_cursed and relic_data.is_cursed:
			return false
		if not include_only_for_events and relic_data.only_for_events:
			return false
		return true
	)

	candidates.shuffle()

	var result: Array[RelicData] = []
	for relic_data in candidates:
		if result.size() >= amount:
			break
		result.append(relic_data)

	return result
# ---------------------------------------------------------
# EVENTS API
# ---------------------------------------------------------

func get_all_events() -> Array[EventData]:
	var result: Array[EventData] = []
	_append_deep_copies(events, result)
	return result

# ---------------------------------------------------------
# CONSUMABLES API
# ---------------------------------------------------------

func get_consumable_by_id(consumable_id: String) -> ConsumableData:
	for consumable in consumables:
		if consumable.id == consumable_id:
			return _duplicate_resource(consumable) as ConsumableData
	return null

func get_all_consumables() -> Array[ConsumableData]:
	var result: Array[ConsumableData] = []
	_append_deep_copies(consumables, result)
	return result

func get_all_consumables_of_type(consumable_type: Consumable.Type) -> Array[ConsumableData]:
	var filtered: Array[ConsumableData] = consumables.filter(func(data: ConsumableData):
		return data.consumable_type == consumable_type
	)

	var result: Array[ConsumableData] = []
	_append_deep_copies(filtered, result)
	return result

# ---------------------------------------------------------
# ENEMY DEBUFFS API
# ---------------------------------------------------------

func get_debuff_data(type: EnemyDebuff.Type) -> EnemyDebuffData:
	for debuff_data in enemy_debuffs:
		if debuff_data.debuff_type == type:
			return _duplicate_resource(debuff_data) as EnemyDebuffData
	return null

# ---------------------------------------------------------
# MAP PIECES API
# ---------------------------------------------------------

func get_all_initial_map_pieces() -> Array[MapPieceData]:
	var result: Array[MapPieceData] = []
	_append_deep_copies(initial_map_pieces, result)
	return result

func get_all_map_pieces() -> Array[MapPieceData]:
	var result: Array[MapPieceData] = []
	_append_deep_copies(map_pieces, result)
	return result

# ---------------------------------------------------------
# ENEMIES API
# ---------------------------------------------------------

## Returns all loaded enemy data resources.
func get_all_enemies() -> Array[EnemyData]:
	var result: Array[EnemyData] = []
	_append_deep_copies(enemy_data.get_all_enemies(), result)
	return result

## Returns enemies filtered by their pressure type.
func get_enemies_by_type(type: EnemyData.Type) -> Array[EnemyData]:
	var result: Array[EnemyData] = []
	_append_deep_copies(enemy_data.get_enemies_by_type(type), result)
	return result

## Returns all non-boss enemies (for regular wave composition).
func get_spawnable_enemies() -> Array[EnemyData]:
	var result: Array[EnemyData] = []
	_append_deep_copies(enemy_data.get_spawnable_enemies(), result)
	return result

# ---------------------------------------------------------
# TOWERS API
# ---------------------------------------------------------

func get_all_tower_data() -> Array[TowerDataWithInstance]:
	var result: Array[TowerDataWithInstance] = []
	_append_deep_copies(tower_data, result)
	return result
	
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

func _load_enemy_debuffs() -> void:
	var loaded_array = _load_resources_from_dir(ENEMY_DEBUFFS_DATA_PATH)
	for data in loaded_array:
		if data is EnemyDebuffData:
			enemy_debuffs.append(data)
		else:
			push_error("[DataLoader] Loaded enemy debuff data has invalid type: %s" % [data])

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

func _append_deep_copies(source: Array, target: Array) -> void:
	for item in source:
		target.append(_duplicate_resource(item))

func _duplicate_resource(resource: Resource) -> Resource:
	return resource.duplicate(true)
