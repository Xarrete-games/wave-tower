class_name FoundationBreaker extends Consumable

func on_click() -> void:
	pass

func use(tile: LevelTileMap) -> void:
	var map_coords: Vector2i = tile.get_mouse_tile_pos()
	tile.unblock_tile(map_coords)
	used.emit(self)
