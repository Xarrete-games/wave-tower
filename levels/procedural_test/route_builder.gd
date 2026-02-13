class_name RouteBuilder
extends RefCounted

## Construye rutas y waypoints para enemigos usando el grafo de conexiones.

var connection_graph: PieceConnectionGraph
var target_piece: MapPiece  # La pieza objetivo (init_piece)


func _init(graph: PieceConnectionGraph, target: MapPiece) -> void:
	connection_graph = graph
	target_piece = target


## Construye la ruta de piezas desde un spawn point hasta el target.
## Retorna Array[MapPiece] ordenado: [spawn_piece, ..., target_piece]
func build_route_to_target(spawn_entry: Dictionary) -> Array[MapPiece]:
	if not spawn_entry.has("piece"):
		push_error("[RouteBuilder] spawn_entry no tiene 'piece'")
		return []
	
	var start_piece: MapPiece = spawn_entry["piece"]
	if start_piece == null or not is_instance_valid(start_piece):
		push_error("[RouteBuilder] spawn_entry.piece no es válido")
		return []
	
	if start_piece == target_piece:
		return [target_piece]
	
	return connection_graph.find_path(start_piece, target_piece)


## Genera waypoints (Vector2 global) para una ruta de piezas.
## Retorna Array[Vector2] con los puntos en orden:
## [spawn_pos, entrada_pieza1, centro_pieza1, salida_pieza1, ..., target]
func build_waypoints_from_route(spawn_entry: Dictionary, route: Array[MapPiece]) -> Array[Vector2]:
	var waypoints: Array[Vector2] = []
	
	if route.size() == 0:
		push_warning("[RouteBuilder] Ruta vacía, no se pueden generar waypoints")
		return waypoints
	
	# 1. Añadir el punto de spawn como primer waypoint
	if spawn_entry.has("pos"):
		waypoints.append(spawn_entry["pos"])
	
	# 2. Para cada pieza de la ruta, generar: entrada, centro, [salida]
	for i in range(route.size()):
		var piece: MapPiece = route[i]
		var entry_dir: MapPiece.Dir = MapPiece.Dir.NE
		var exit_dir: MapPiece.Dir = MapPiece.Dir.NE
		var has_exit: bool = (i < route.size() - 1)
		
		# Determinar dirección de entrada
		if i == 0:
			if spawn_entry.has("dir"):
				entry_dir = spawn_entry["dir"]
		else:
			var prev_piece: MapPiece = route[i - 1]
			entry_dir = connection_graph.find_connection_dir(prev_piece, piece)
			entry_dir = GridManager.get_opposite_dir(entry_dir)
		
		# Determinar dirección de salida (si hay siguiente pieza)
		if has_exit:
			var next_piece: MapPiece = route[i + 1]
			exit_dir = connection_graph.find_connection_dir(piece, next_piece)
		
		# Generar waypoints para esta pieza
		var entry_local: Vector2 = piece.get_edge_tile_pos(entry_dir)
		var entry_global: Vector2 = piece.global_position + entry_local
		waypoints.append(entry_global)
		
		waypoints.append(piece.global_position)
		
		if has_exit:
			var exit_local: Vector2 = piece.get_edge_tile_pos(exit_dir)
			var exit_global: Vector2 = piece.global_position + exit_local
			waypoints.append(exit_global)
	
	# 3. Añadir el target final
	if target_piece != null:
		waypoints.append(target_piece.get_target())
	
	return waypoints


## Función de conveniencia: dado un spawn_entry, retorna los waypoints completos.
func get_waypoints_for_spawn(spawn_entry: Dictionary) -> Array[Vector2]:
	var route: Array[MapPiece] = build_route_to_target(spawn_entry)
	return build_waypoints_from_route(spawn_entry, route)
