class_name RouteBuilder
extends RefCounted

## Builds routes and waypoints for enemies using the connection graph.

var connection_graph: PieceConnectionGraph
var target_piece: MapPiece  # Target piece (init_piece)


func _init(graph: PieceConnectionGraph, target: MapPiece) -> void:
	connection_graph = graph
	target_piece = target


## Builds the piece route from a spawn point to the target.
## Returns Array[MapPiece] ordered: [spawn_piece, ..., target_piece]
func build_route_to_target(spawn_entry: Dictionary) -> Array[MapPiece]:
	if not spawn_entry.has("piece"):
		push_error("[RouteBuilder] spawn_entry has no 'piece'")
		return []
	
	var start_piece: MapPiece = spawn_entry["piece"]
	if start_piece == null or not is_instance_valid(start_piece):
		push_error("[RouteBuilder] spawn_entry.piece is not valid")
		return []
	
	if start_piece == target_piece:
		return [target_piece]
	
	return connection_graph.find_path(start_piece, target_piece)


## Generates waypoints (global Vector2) for a piece route.
## Returns Array[Vector2] with points in order:
## [spawn_pos, piece1_entry, piece1_center, piece1_exit, ..., target]
func build_waypoints_from_route(spawn_entry: Dictionary, route: Array[MapPiece]) -> Array[Vector2]:
	var waypoints: Array[Vector2] = []
	
	if route.size() == 0:
		push_warning("[RouteBuilder] Empty route, cannot generate waypoints")
		return waypoints
	
	# 1. Add spawn point as first waypoint
	if spawn_entry.has("pos"):
		waypoints.append(spawn_entry["pos"])
	
	# 2. For each piece in route, generate: entry, center, [exit]
	for i in range(route.size()):
		var piece: MapPiece = route[i]
		var entry_dir: Edge.Dir = Edge.Dir.NE
		var exit_dir: Edge.Dir = Edge.Dir.NE
		var has_exit: bool = (i < route.size() - 1)
		
		# Determine entry direction
		if i == 0:
			if spawn_entry.has("edge"):
				entry_dir = spawn_entry["edge"].dir
		else:
			var prev_piece: MapPiece = route[i - 1]
			entry_dir = connection_graph.find_connection_dir(prev_piece, piece)
			entry_dir = Edge.get_opposite_dir(entry_dir)
		
		# Determine exit direction (if there's a next piece)
		if has_exit:
			var next_piece: MapPiece = route[i + 1]
			exit_dir = connection_graph.find_connection_dir(piece, next_piece)
		
		# Generate waypoints for this piece
		var entry_local: Vector2 = piece.get_edge_tile_pos(entry_dir)
		var entry_global: Vector2 = piece.global_position + entry_local
		waypoints.append(entry_global)
		
		waypoints.append(piece.global_position)
		
		if has_exit:
			var exit_local: Vector2 = piece.get_edge_tile_pos(exit_dir)
			var exit_global: Vector2 = piece.global_position + exit_local
			waypoints.append(exit_global)
	
	# 3. Add final target
	if target_piece != null:
		waypoints.append(target_piece.get_target())
	
	return waypoints


## Convenience function: given a spawn_entry, returns complete waypoints.
func get_waypoints_for_spawn(spawn_entry: Dictionary) -> Array[Vector2]:
	var route: Array[MapPiece] = build_route_to_target(spawn_entry)
	return build_waypoints_from_route(spawn_entry, route)
