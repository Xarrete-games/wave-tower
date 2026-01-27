class_name WorldMap extends Node2D

@export var piece_a: MapPiece
@export var piece_b: MapPiece
@export var enemy: EnemyProcedural


var paths: Array[PiecePath] = []

func _ready() -> void:
	paths.append(piece_a.get_path_piece())
	paths.append(piece_b.get_path_piece())
	attach_piece(piece_a, piece_b)
	enemy.path_queue = paths
	enemy.advance_to_next_path()

func attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece) -> void:
	var a_tile := p_piece_a.get_edge_tile(p_piece_a.exit_dir)
	var b_tile := p_piece_b.get_edge_tile(p_piece_b.entry_dir)

	var a_world = p_piece_a.map_to_local(a_tile)
	var b_world = p_piece_b.map_to_local(b_tile)

	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world
