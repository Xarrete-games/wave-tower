class_name LevelTileMap extends TileMapLayer

const BUILDEABLE = "buildeable"
const BLOCKED = "blocked"
const ATLAS_ID = 0

const UNLOCK_TILE_POS = Vector2i(6, 0)
const NORMAL_TILE_POS = Vector2i(2, 0)

var _occupied_tiles: Dictionary[Vector2i, bool] = {}
var _blocked_tiles: Dictionary[Vector2i, bool] = {}
var _buildeable_tiles: Dictionary[Vector2i, bool] = {}

func _ready() -> void:
	ClickEvents.tower_remove_pressed.connect(_on_tower_removed)
	RunContext.level_tile_map = self
	_fill_data()
	
func get_mouse_tile_pos() -> Vector2i:
	var mouse_pos = get_global_mouse_position()
	return local_to_map(to_local(mouse_pos))

func is_mouse_on_block_tile() -> bool:
	var map_coords: Vector2i = get_mouse_tile_pos()
	var tile_data = get_cell_tile_data(map_coords)
	
	if tile_data == null:
		return false
	
	if tile_data.get_custom_data(BLOCKED) == true:
		return true
		
	return false

func is_mouse_on_buildeable_tile() -> bool:
	var map_coords: Vector2i = get_mouse_tile_pos()

	#chekc if is buildeable
	if not _buildeable_tiles.has(map_coords):
		return false
	
	#check if tile is occupied or bloqued
	if _occupied_tiles.has(map_coords) or _blocked_tiles.has(map_coords):
		return false
		
	return true

func destroy_random_buildeable_tile() -> void:
	var buildeable_tiles_array = _buildeable_tiles.keys()
	if buildeable_tiles_array.is_empty():
		return
	var rand_index = randi() % buildeable_tiles_array.size()
	var tile_to_block = buildeable_tiles_array[rand_index]
	_occupied_tiles[tile_to_block] = true
	set_cell(tile_to_block, ATLAS_ID, NORMAL_TILE_POS)
	_buildeable_tiles.erase(tile_to_block)

func get_current_tile_pos() -> Vector2:
	var center_pos_local = map_to_local(get_mouse_tile_pos())
	# fix 96 px tile center	
	center_pos_local.y -= 16
	return to_global(center_pos_local)

func set_tile_occupied(map_coords: Vector2i):
	_occupied_tiles[map_coords] = true
	_buildeable_tiles.erase(map_coords)

func set_tile_free(map_coords: Vector2i):
	if _occupied_tiles.has(map_coords):
		_occupied_tiles.erase(map_coords)

func unblock_tile(map_coords: Vector2i) -> void:
	# remove first
	var blocked_tiles_array = _blocked_tiles.keys()
	if blocked_tiles_array.is_empty():
		return
	_blocked_tiles.erase(map_coords)
	set_cell(map_coords, ATLAS_ID, UNLOCK_TILE_POS)
		
func _fill_data() -> void:
	var used_cells: Array[Vector2i] = get_used_cells()
	
	for map_coords: Vector2i in used_cells:
		var tile_data = get_cell_tile_data(map_coords)
		if tile_data != null and tile_data.get_custom_data(BLOCKED) == true:
			_blocked_tiles[map_coords] = true
		elif tile_data != null and tile_data.get_custom_data(BUILDEABLE) == true:
			_buildeable_tiles[map_coords] = true

func _on_tower_removed(tower: Tower) -> void:
	var tile = tower.tile_pos
	set_tile_free(tile)
	
