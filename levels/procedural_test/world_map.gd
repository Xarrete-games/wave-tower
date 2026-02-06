class_name WorldMap extends Node2D

@export var init_map_piece_data: MapPieceData
@export var enemy: EnemyProcedural


var map_pieces: Array[MapPieceData] = []
var last_piece_attached: MapPiece = null
#grid
var gird_offsets: Dictionary[MapPiece.Dir, Vector2i] = {
	MapPiece.Dir.NE: Vector2i(1, -1),
	MapPiece.Dir.SE: Vector2i(1, 0),
	MapPiece.Dir.SW: Vector2i(-1, 1),
	MapPiece.Dir.NW: Vector2i(-1, 0)
}
var all_dirs: Array[MapPiece.Dir] = [MapPiece.Dir.NE, MapPiece.Dir.SE, MapPiece.Dir.SW, MapPiece.Dir.NW]
var grid: Dictionary[Vector2i, bool] = {}
var current_tile: Vector2i = Vector2i.ZERO


func _ready() -> void:
	var init_piece: MapPiece = init_map_piece_data.get_instance()
	map_pieces = DataLoader.get_all_map_pieces()
	grid[Vector2i.ZERO] = true
	current_tile = Vector2i.ZERO
	add_child(init_piece)

	last_piece_attached = init_piece

func _input(event: InputEvent) -> void:
	if event.is_action("test"):
		attach_next_piece()

func attach_next_piece() -> void:
	if last_piece_attached == null:
		push_error("No piece to attach")
		return

	# next piece must connect to a valid random edge of the current piece
	var next_dir: MapPiece.Dir = last_piece_attached.edges.pick_random()
	var dir_to_connect = get_dir_to_connect(next_dir)

	# grid
	current_tile = _calculate_next_tile_pos(next_dir)
	grid[current_tile] = true

	var invalid_edges = get_invalid_edges(dir_to_connect)

	var valid_pieces = map_pieces.filter(func(p: MapPieceData):
		return p.edges.has(dir_to_connect) and not invalid_edges.any(func(e): return p.edges.has(e))
	)

	if valid_pieces.size() == 0:
		push_error("No valid pieces to attach to edge %s" % [next_dir])
		return

	var piece_data: MapPieceData = valid_pieces[randi() % valid_pieces.size()]
	var new_piece: MapPiece = piece_data.get_instance()
	add_child(new_piece)

	attach_piece(last_piece_attached, new_piece, next_dir, dir_to_connect)
	new_piece.set_edge_has_connected(dir_to_connect)
	last_piece_attached = new_piece
	

func attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece, entry_dir: MapPiece.Dir, exit_dir: MapPiece.Dir) -> void:
	var a_world = p_piece_a.get_edge_tile_pos(entry_dir)
	var b_world = p_piece_b.get_edge_tile_pos(exit_dir)
	# compute tile-based offset so edges are adjacent (works for isometric)
	var delta: Vector2i = p_piece_a.get_edge_tile_delta(entry_dir)
	var shift: Vector2 = p_piece_a.get_tile_local_offset(delta)

	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world + shift

func get_dir_to_connect(dir: MapPiece.Dir) -> MapPiece.Dir:
	match dir:
		MapPiece.Dir.NE:
			return MapPiece.Dir.SW
		MapPiece.Dir.SE:
			return MapPiece.Dir.NW
		MapPiece.Dir.SW:
			return MapPiece.Dir.NE
		MapPiece.Dir.NW:
			return MapPiece.Dir.SE
		_:
			push_error("Invalid direction: %s" % [dir])
			return MapPiece.Dir.NE


func _calculate_next_tile_pos(next_dir: MapPiece.Dir) -> Vector2i:
	current_tile += gird_offsets[next_dir]
	return current_tile

func get_invalid_edges(dir_to_connect: MapPiece.Dir) -> Array[MapPiece.Dir]:

	var dirs_check = all_dirs.duplicate().filter(func(d): return d != dir_to_connect)
	var invalid_dirs: Array[MapPiece.Dir] = []
	

	for dir in dirs_check:
		var check_tile = current_tile + gird_offsets[dir]
		if grid.has(check_tile):
			invalid_dirs.append(dir)
	
	return invalid_dirs
