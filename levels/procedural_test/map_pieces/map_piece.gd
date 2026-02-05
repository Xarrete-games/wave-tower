class_name MapPiece extends Node2D

enum Dir { NE, SE, SW, NW }

const size: Vector2i = Vector2i(11, 11)

var edges: Array[Dir] = []

@onready var tile_map: TileMapLayer = $MapPieceTileMap

# Get the tile position of the edge in the given direction
func get_edge_tile_pos(dir: Dir) -> Vector2:
	var used: Array[Vector2i] = tile_map.get_used_cells()
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
	var sum: Vector2i = Vector2i.ZERO
	for c in edge:
		sum += c

	var tile = sum / edge.size()
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
	return tile_map.map_to_local(tile_pos)
