class_name PieceConnectionGraph
extends RefCounted

## Map piece connection graph.
## Allows registering bidirectional connections and finding paths between pieces.

# Structure: { MapPiece: { Dir: MapPiece } }
var connections: Dictionary[MapPiece, Dictionary] = {}

func _init() -> void:
	connections = {}

## Initializes an entry for a piece (no connections yet).
func register_piece(piece: MapPiece) -> void:
	if not connections.has(piece):
		connections[piece] = {}

## Registers a bidirectional connection between two pieces.
func connect_pieces(piece_a: MapPiece, piece_b: MapPiece, dir_a: MapPiece.Dir, dir_b: MapPiece.Dir) -> void:
	if not connections.has(piece_a):
		connections[piece_a] = {}
	if not connections.has(piece_b):
		connections[piece_b] = {}
	
	connections[piece_a][dir_a] = piece_b
	connections[piece_b][dir_b] = piece_a

## Gets all connections of a piece.
func get_connections(piece: MapPiece) -> Dictionary[MapPiece.Dir, MapPiece]:
	if connections.has(piece):
		return connections[piece]
	return {}

## Finds the direction in from_piece that connects to to_piece.
func find_connection_dir(from_piece: MapPiece, to_piece: MapPiece) -> MapPiece.Dir:
	if not connections.has(from_piece):
		push_warning("[PieceConnectionGraph] from_piece has no registered connections")
		return MapPiece.Dir.NE
	
	var piece_connections: Dictionary[MapPiece.Dir, MapPiece] = connections[from_piece]
	for dir in piece_connections.keys():
		if piece_connections[dir] == to_piece:
			return dir
	
	push_warning("[PieceConnectionGraph] No connection found from %s to %s" % [from_piece, to_piece])
	return MapPiece.Dir.NE


## BFS to find path between two pieces.
## Returns Array[MapPiece] ordered from from_piece to to_piece.
func find_path(from_piece: MapPiece, to_piece: MapPiece) -> Array[MapPiece]:
	if from_piece == to_piece:
		return [from_piece]
	
	var queue: Array[MapPiece] = [from_piece]
	var came_from: Dictionary[MapPiece, MapPiece] = {}
	came_from[from_piece] = null
	
	while queue.size() > 0:
		var current: MapPiece = queue.pop_front()
		
		if not connections.has(current):
			continue
		
		var piece_connections: Dictionary[MapPiece.Dir, MapPiece] = connections[current]
		for dir in piece_connections.keys():
			var neighbor: MapPiece = piece_connections[dir]
			if neighbor == null or not is_instance_valid(neighbor):
				continue
			if came_from.has(neighbor):
				continue
			
			came_from[neighbor] = current
			
			if neighbor == to_piece:
				return _reconstruct_path(came_from, to_piece)
			
			queue.append(neighbor)
	
	push_warning("[PieceConnectionGraph] No route found from %s to %s" % [from_piece, to_piece])
	return []


func _reconstruct_path(came_from: Dictionary[MapPiece, MapPiece], end: MapPiece) -> Array[MapPiece]:
	var path: Array[MapPiece] = []
	var current: MapPiece = end
	
	while current != null:
		path.append(current)
		if came_from.has(current):
			current = came_from[current]
		else:
			break
	
	path.reverse()
	return path
