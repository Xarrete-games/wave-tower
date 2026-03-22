class_name FoundationBreaker extends ConsumableTargeteable

func action(tile: Variant) -> void:
	var tile_map = tile as CompositeTileMap
	tile_map.unblock_tile_at_mouse()
	
