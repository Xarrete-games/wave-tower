class_name MapPiece extends TileMapLayer

enum Dir { NE, SE, SW, NW }
const DIR_TO_OFFSET: Dictionary= {
	Dir.NE: Vector2i( 1, -1),
	Dir.SE: Vector2i( 1,  1),
	Dir.SW: Vector2i(-1,  1),
	Dir.NW: Vector2i(-1, -1),
}	

@export var size: Vector2i = Vector2i(11, 11)
@export var entrey_dir: Dir
@export var exit_dir: Dir
@export var paths: Array[PiecePath] = []

var edges_tiles: Array[Vector2i] = []

func _ready() -> void:
	pass

func get_edge_center(dir: Dir) -> Vector2i:
	var used: Array[Vector2i] = get_used_cells()
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

	return sum / edge.size()

func get_map_center() -> Vector2i:
	var used: Array[Vector2i] = get_used_cells()
	var sum: Vector2i = Vector2i.ZERO
	for c in used:
		sum += c
	return sum / used.size()