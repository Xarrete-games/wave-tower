class_name WorldMap extends Node2D

@export var init_map_piece_data: MapPieceData
@export var enemy: EnemyProcedural

var map_pieces: Array[MapPieceData] = []
var next_piece_to_attach: MapPiece = null

func _ready() -> void:
	var init_piece: MapPiece = init_map_piece_data.get_instance()
	map_pieces = DataLoader.get_all_map_pieces()

	add_child(init_piece)

	next_piece_to_attach = init_piece

func _input(event: InputEvent) -> void:
	if event.is_action("test"):
		attach_next_piece()

func attach_next_piece() -> void:
	if next_piece_to_attach == null:
		push_error("No piece to attach")
		return

	# next piece must connect to a valid random edge of the current piece
	var next_dir: MapPiece.Dir = next_piece_to_attach.edges.pick_random()
	var dir_to_connect = get_dir_to_connect(next_dir)

	var valid_pieces = map_pieces.filter(func(p: MapPieceData):
		return p.edges.has(dir_to_connect)
	)

	if valid_pieces.size() == 0:
		push_error("No valid pieces to attach to edge %s" % [next_dir])
		return

	var piece_data: MapPieceData = valid_pieces[randi() % valid_pieces.size()]
	var new_piece: MapPiece = piece_data.get_instance()
	add_child(new_piece)

	attach_piece(next_piece_to_attach, new_piece, next_dir, dir_to_connect)
	new_piece.set_edge_has_connected(dir_to_connect)
	next_piece_to_attach = new_piece

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
