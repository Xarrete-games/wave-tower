class_name LevelTileMap extends TileMapLayer

const BUILDEABLE = "buildeable"
const BLOCKED = "blocked"
const ATLAS_ID = 0
const LEVEL1_UNLOCKED_TILE = Vector2i(0, 0)
const LEVEL2_UNLOCKED_TILE = Vector2i(0, 0)
const LEVEL3_UNLOCKED_TILE = Vector2i(0, 0)

@export var level: int = 1

var unlocked_tiles: Dictionary[int, Vector2i] = {
	1: LEVEL1_UNLOCKED_TILE,
	2: LEVEL2_UNLOCKED_TILE,
	3: LEVEL3_UNLOCKED_TILE,
}
var _occupied_tiles: Dictionary[Vector2i, bool] = {}
var _blocked_tiles: Dictionary[Vector2i, bool] = {}

func _ready() -> void:
	TowerPlacementManager.tower_sold.connect(_on_tower_sold)
	_fill_blocked_dic()
	
func get_mouse_tile_pos() -> Vector2i:
	var mouse_pos = get_global_mouse_position()
	return local_to_map(to_local(mouse_pos))

func is_mouse_on_buildeable_tile() -> bool:
	var map_coords: Vector2i = get_mouse_tile_pos()
	var tile_data = get_cell_tile_data(map_coords)
	
	#chekc if is buildeable
	if tile_data == null or not tile_data.get_custom_data("buildeable"):
		return false
	
	#check if tile is occupied or bloqued
	if _occupied_tiles.has(map_coords) or _blocked_tiles.has(map_coords):
		return false
		
	return true

func get_current_tile_pos() -> Vector2:
	var center_pos_local = map_to_local(get_mouse_tile_pos())
	# fix 96 px tile center	
	center_pos_local.y -= 16
	return to_global(center_pos_local)

func set_tile_occupied(map_coords: Vector2i):
	_occupied_tiles[map_coords] = true

func set_tile_free(map_coords: Vector2i):
	if _occupied_tiles.has(map_coords):
		_occupied_tiles.erase(map_coords)

func unblock_tile() -> void:
	# remove first
	var blocked_tiles_array = _blocked_tiles.keys()
	if blocked_tiles_array.size() > 0:
		var coords: Vector2i = blocked_tiles_array[0]
		_blocked_tiles.erase(coords)
		set_cell(coords, ATLAS_ID, unlocked_tiles[level])
		
func _fill_blocked_dic() -> void:
	var used_cells: Array[Vector2i] = get_used_cells()
	
	for map_coords: Vector2i in used_cells:
		var tile_data = get_cell_tile_data(map_coords)
		if tile_data != null and tile_data.get_custom_data(BLOCKED) == true:
			_blocked_tiles[map_coords] = true

func _on_tower_sold(tower: Tower) -> void:
	var tile = tower.tile_pos
	set_tile_free(tile)
		
