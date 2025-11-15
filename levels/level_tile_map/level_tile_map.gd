class_name LevelTileMap extends TileMapLayer

var _occupied_tiles: Dictionary = {}

func get_mouse_tile_pos() -> Vector2i:
	var mouse_pos = get_global_mouse_position()
	return local_to_map(to_local(mouse_pos))

func is_mouse_on_buildeable_tile() -> bool:
	var map_coords: Vector2i = get_mouse_tile_pos()
	var tile_data = get_cell_tile_data(map_coords)
	
	#chekc if is buildeable
	if tile_data == null or not tile_data.get_custom_data("buildeable"):
		return false
	
	#check if tower exist in the tile
	if _occupied_tiles.has(map_coords):
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

func _ready():
	get_used_cells()
		
