class_name PieceConnectionGraph
extends RefCounted

## Grafo de conexiones entre piezas del mapa.
## Permite registrar conexiones bidireccionales y buscar caminos entre piezas.

# Estructura: { MapPiece: { Dir: MapPiece } }
var connections: Dictionary = {}


func _init() -> void:
	connections = {}


## Inicializa una entrada para una pieza (sin conexiones aún).
func register_piece(piece: MapPiece) -> void:
	if not connections.has(piece):
		connections[piece] = {}


## Registra una conexión bidireccional entre dos piezas.
func connect_pieces(piece_a: MapPiece, piece_b: MapPiece, dir_a: MapPiece.Dir, dir_b: MapPiece.Dir) -> void:
	if not connections.has(piece_a):
		connections[piece_a] = {}
	if not connections.has(piece_b):
		connections[piece_b] = {}
	
	connections[piece_a][dir_a] = piece_b
	connections[piece_b][dir_b] = piece_a


## Obtiene todas las conexiones de una pieza.
func get_connections(piece: MapPiece) -> Dictionary:
	if connections.has(piece):
		return connections[piece]
	return {}


## Encuentra la dirección en from_piece que conecta con to_piece.
func find_connection_dir(from_piece: MapPiece, to_piece: MapPiece) -> MapPiece.Dir:
	if not connections.has(from_piece):
		push_warning("[PieceConnectionGraph] from_piece no tiene conexiones registradas")
		return MapPiece.Dir.NE
	
	var piece_connections: Dictionary = connections[from_piece]
	for dir in piece_connections.keys():
		if piece_connections[dir] == to_piece:
			return dir
	
	push_warning("[PieceConnectionGraph] No se encontró conexión de %s a %s" % [from_piece, to_piece])
	return MapPiece.Dir.NE


## BFS para encontrar el camino entre dos piezas.
## Retorna Array[MapPiece] ordenado desde from_piece hasta to_piece.
func find_path(from_piece: MapPiece, to_piece: MapPiece) -> Array[MapPiece]:
	if from_piece == to_piece:
		return [from_piece]
	
	var queue: Array[MapPiece] = [from_piece]
	var came_from: Dictionary = {}
	came_from[from_piece] = null
	
	while queue.size() > 0:
		var current: MapPiece = queue.pop_front()
		
		if not connections.has(current):
			continue
		
		var piece_connections: Dictionary = connections[current]
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
	
	push_warning("[PieceConnectionGraph] No se encontró ruta desde %s hasta %s" % [from_piece, to_piece])
	return []


func _reconstruct_path(came_from: Dictionary, end: MapPiece) -> Array[MapPiece]:
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
