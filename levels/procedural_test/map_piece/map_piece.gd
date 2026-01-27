class_name MapPiece extends Node2D

enum Dir { NE, SE, SW, NW }

@export var size: Vector2i = Vector2i(11, 11)
@export var entry_dir: Dir
@export var exit_dir: Dir

@onready var tile_map: TileMapLayer = $MapPieceTileMap
@onready var path_piece: PiecePath = $PiecePath

func _ready() -> void:
	pass


func get_path_piece() -> PiecePath:
	return path_piece

func map_to_local(tile_pos: Vector2i) -> Vector2:
	return tile_map.map_to_local(tile_pos)

func get_edge_tile(dir: Dir) -> Vector2i:
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

	return sum / edge.size()
