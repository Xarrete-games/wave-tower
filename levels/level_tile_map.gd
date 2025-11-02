class_name LevelTileMap extends TileMapLayer

func is_mouse_on_buildeable_tile() -> bool:
	var tile_data =  get_cell_tile_data(_get_mouse_coords())
	return tile_data.get_custom_data("buildeable")

func get_current_tile_pos() -> Vector2:
	var center_pos_local = map_to_local(_get_mouse_coords())
	return to_global(center_pos_local)

func _ready():
	pass
		
func _get_mouse_coords() -> Vector2:
	var mouse_pos = get_global_mouse_position()
	return local_to_map(to_local(mouse_pos))
