class_name FoundationBreaker extends ConsumableTargeteable

func action(tile: Variant) -> void:
	var tile_map = tile as LevelTileMap
	var map_coords: Vector2i = tile_map.get_mouse_tile_pos()
	tile_map.unblock_tile(map_coords)
	
