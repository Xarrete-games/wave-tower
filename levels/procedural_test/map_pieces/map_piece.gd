class_name MapPiece extends Node2D

enum Dir { NE, SE, SW, NW }

const size: Vector2i = Vector2i(11, 11)

var edges: Array[Dir] = []
var debug_edge_pos: bool = true

@onready var tile_map: TileMapLayer = $MapPieceTileMap

# Get the tile position of the edge in the given direction
func get_edge_tile_pos(dir: Dir) -> Vector2:
	var used: Array[Vector2i] = tile_map.get_used_cells()
	if used.size() == 0:
		push_error("TileMap has no used cells")
		return Vector2.ZERO

	var edge: Array[Vector2i] = []

	match dir:
		Dir.NE:
			var min_y = used.map(func(c): return c.y).min()
			edge = used.filter(func(c): return c.y == min_y)

		Dir.SW:
			var max_y = used.map(func(c): return c.y).max()
			edge = used.filter(func(c): return c.y == max_y)

		Dir.NW:
			var min_x = used.map(func(c): return c.x).min()
			edge = used.filter(func(c): return c.x == min_x)

		Dir.SE:
			var max_x = used.map(func(c): return c.x).max()
			edge = used.filter(func(c): return c.x == max_x)

	# get center of edge
	if edge.size() == 0:
		push_error("Edge has no tiles for dir %s" % [dir])
		return Vector2.ZERO

	var sum: Vector2i = Vector2i.ZERO
	for c in edge:
		sum += c

	var tile = sum / edge.size()

	if debug_edge_pos:
		push_warning("get_edge_tile_pos %s used=%s edge=%s tile=%s local=%s" % [dir, used, edge, tile, _map_to_local(tile)])

	return _map_to_local(tile)

func set_edge_has_connected(dir: Dir) -> void:
	if edges.size() == 0:
		push_error("Trying to set edge %s as connected, but this piece has no edges" % [dir])
		return

	if not edges.has(dir):
		push_error("Trying to set edge %s as connected, but it is not an edge in this piece" % [dir])
		return

	edges.erase(dir)

func _map_to_local(tile_pos: Vector2i) -> Vector2:
	var local_in_tilemap: Vector2 = tile_map.map_to_local(tile_pos)
	var global_point: Vector2 = tile_map.to_global(local_in_tilemap)
	return to_local(global_point)

func get_edge_normal(dir: Dir) -> Vector2:
	match dir:
		Dir.NE:
			return Vector2(0, -1)
		Dir.SE:
			return Vector2(1, 0)
		Dir.SW:
			return Vector2(0, 1)
		Dir.NW:
			return Vector2(-1, 0)
		_:
			return Vector2.ZERO

func get_edge_tile_delta(dir: Dir) -> Vector2i:
	match dir:
		Dir.NE:
			return Vector2i(0, -1)
		Dir.SE:
			return Vector2i(1, 0)
		Dir.SW:
			return Vector2i(0, 1)
		Dir.NW:
			return Vector2i(-1, 0)
		_:
			return Vector2i(0, 0)

func get_tile_local_offset(delta: Vector2i) -> Vector2:
	var origin = _map_to_local(Vector2i(0, 0))
	var target = _map_to_local(delta)
	return target - origin
