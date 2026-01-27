class_name WorldMap extends Node2D

const DIR_TO_OFFSET: Dictionary= {
	MapPiece.Dir.NE: Vector2i( 1, -1),
	MapPiece.Dir.SE: Vector2i( 1,  1),
	MapPiece.Dir.SW: Vector2i(-1,  1),
	MapPiece.Dir.NW: Vector2i(-1, -1),
}	

@export var piece_a: MapPiece
@export var piece_b: MapPiece


func _ready() -> void:
	attach_piece(piece_a, piece_b)


func attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece) -> void:
	var a_tile := p_piece_a.get_edge_tile(p_piece_a.exit_dir)
	var b_tile := p_piece_b.get_edge_tile(p_piece_b.entry_dir)

	var a_world = p_piece_a.map_to_local(a_tile)
	var b_world = p_piece_b.map_to_local(b_tile)

	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world
