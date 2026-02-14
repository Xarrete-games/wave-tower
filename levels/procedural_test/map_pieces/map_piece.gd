class_name MapPiece extends Node2D

# only for target portal
@export var target_portal: Node2D = null

const size: Vector2i = Vector2i(11, 11)

var edges: Array[Edge] = []
var logical_pos: Vector2i = Vector2i.ZERO


@onready var tile_map: TileMapLayer = $MapPieceTileMap

func get_target()  -> Vector2:
	if target_portal:
		return target_portal.global_position
	else:
		return global_position

# Get the tile position of the edge in the given direction
func get_edge_tile_pos(dir: Edge.Dir) -> Vector2:
	var used: Array[Vector2i] = tile_map.get_used_cells()
	if used.size() == 0:
		push_error("TileMap has no used cells")
		return Vector2.ZERO

	var edge: Array[Vector2i] = []

	match dir:
		Edge.Dir.NE:
			var min_y = used.map(func(c): return c.y).min()
			edge = used.filter(func(c): return c.y == min_y)

		Edge.Dir.SW:
			var max_y = used.map(func(c): return c.y).max()
			edge = used.filter(func(c): return c.y == max_y)

		Edge.Dir.NW:
			var min_x = used.map(func(c): return c.x).min()
			edge = used.filter(func(c): return c.x == min_x)

		Edge.Dir.SE:
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

	return _map_to_local(tile)

func set_edge_has_connected(edge: Edge) -> void:
	if edges.size() == 0:
		push_error("Trying to set edge %s as connected, but this piece has no edges" % [edge])
		return

	var to_remove: Edge = null
	for e in edges:
		if e.matches(edge):
			to_remove = e
			break
	
	if to_remove == null:
		push_error("Trying to set edge %s as connected, but it is not an edge in this piece" % [edge])
		return

	edges.erase(to_remove)


## Finds an edge by direction (returns first match or null)
func find_edge_by_dir(dir: Edge.Dir) -> Edge:
	for e in edges:
		if e.dir == dir:
			return e
	return null


## Checks if piece has an edge with given direction
func has_edge_dir(dir: Edge.Dir) -> bool:
	return find_edge_by_dir(dir) != null

func _map_to_local(tile_pos: Vector2i) -> Vector2:
	var local_in_tilemap: Vector2 = tile_map.map_to_local(tile_pos)
	var global_point: Vector2 = tile_map.to_global(local_in_tilemap)
	return to_local(global_point)

func get_edge_normal(dir: Edge.Dir) -> Vector2:
	match dir:
		Edge.Dir.NE:
			return Vector2(0, -1)
		Edge.Dir.SE:
			return Vector2(1, 0)
		Edge.Dir.SW:
			return Vector2(0, 1)
		Edge.Dir.NW:
			return Vector2(-1, 0)
		_:
			return Vector2.ZERO

func get_edge_tile_delta(dir: Edge.Dir) -> Vector2i:
	match dir:
		Edge.Dir.NE:
			return Vector2i(0, -1)
		Edge.Dir.SE:
			return Vector2i(1, 0)
		Edge.Dir.SW:
			return Vector2i(0, 1)
		Edge.Dir.NW:
			return Vector2i(-1, 0)
		_:
			return Vector2i(0, 0)

func get_tile_local_offset(delta: Vector2i) -> Vector2:
	var origin = _map_to_local(Vector2i(0, 0))
	var target = _map_to_local(delta)
	return target - origin
