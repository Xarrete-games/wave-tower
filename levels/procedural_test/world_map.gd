class_name WorldMap extends Node2D

const ORANGE_PORTAL: PackedScene = preload("uid://b8g0wp8j02vu4")
const PORTAL_OFFSET: Vector2 = Vector2(0, -80)

@export var init_map_piece_data: MapPieceData
@export var enemy: EnemyProcedural
@export var visual: Node2D


var map_pieces: Array[MapPieceData] = []
var last_piece_attached: MapPiece = null
#grid
var grid_offsets: Dictionary[MapPiece.Dir, Vector2i] = {
	MapPiece.Dir.NE: Vector2i(1, -1),
	MapPiece.Dir.SE: Vector2i(1, 0),
	MapPiece.Dir.SW: Vector2i(-1, 1),
	MapPiece.Dir.NW: Vector2i(-1, 0)
}
var all_dirs: Array[MapPiece.Dir] = [MapPiece.Dir.NE, MapPiece.Dir.SE, MapPiece.Dir.SW, MapPiece.Dir.NW]
var grid: Dictionary[Vector2i, bool] = {}
var current_tile: Vector2i = Vector2i.ZERO
var frontiers: Array[MapPiece] = []
var portal_spawn_positions: Array[Vector2] = []
var spawn_handler: SpawnPositionsHandler = null
# entries keep both position and direction so we can flip sprites correctly
var portal_entries: Array[Dictionary] = []
var finalized_portal_entries: Array[Dictionary] = []

# --- Conexiones entre piezas (grafo dirigido) ---
# Estructura: { MapPiece: { Dir: MapPiece } }
# Ejemplo: piece_connections[pieceA][Dir.NE] = pieceB significa que pieceA conecta con pieceB por su borde NE
var piece_connections: Dictionary = {}

# Referencia a la pieza inicial (objetivo/target de los enemigos)
var init_piece: MapPiece = null


func _ready() -> void:
	spawn_handler = SpawnPositionsHandler.new(visual)
	# Instanciar la pieza inicial y guardar referencia (es el target de los enemigos)
	init_piece = init_map_piece_data.get_instance()
	map_pieces = DataLoader.get_all_map_pieces()
	grid[Vector2i.ZERO] = true
	current_tile = Vector2i.ZERO
	add_child(init_piece)
	# logical coord for initial piece
	init_piece.logical_pos = Vector2i.ZERO
	
	# Inicializar entrada en el grafo de conexiones para la pieza inicial
	piece_connections[init_piece] = {}
	
	if init_piece.edges.size() > 0:
		frontiers.append(init_piece)

	last_piece_attached = init_piece

	# crear portales para los edges actuales
	update_portals()

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("test"):
		attach_next_piece()
	# TEST: Pulsar "ui_accept" (Enter/Space) para probar waypoints con el enemy
	if event.is_action_pressed("ui_accept"):
		_test_spawn_enemy_with_waypoints()

func attach_next_piece() -> void:
	# Función principal: coordina un solo paso de crecimiento.
	# Selecciona una frontier (si existe), escoge un único edge aleatorio de esa frontier,
	# valida el candidato y trata de colocar una pieza. Si la colocación tiene éxito,
	# poda los edges bloqueados en todas las frontiers.
	if frontiers.size() == 0:
		push_error("No frontiers available for placement")
		return

	var frontier: MapPiece = _select_frontier()
	if frontier == null:
		push_error("Failed to select frontier")
		return

	# si la frontier no tiene edges, elimínala y sal
	if frontier.edges.size() == 0:
		frontiers.erase(frontier)
		update_portals()
		return

	var next_dir = _pick_random_edge(frontier)
	var candidate_tile = frontier.logical_pos + grid_offsets[next_dir]

	var validation = _validate_edge(frontier, next_dir, candidate_tile)
	if not validation.valid:
		# elimina el edge inválido de la frontier (y la frontier si queda vacía)
		push_warning(validation.reason)
		# guardar punto de spawn definitivo antes de quitar el edge
		finalize_spawn_pos(frontier, next_dir)
		frontier.edges.erase(next_dir)
		if frontier.edges.size() == 0:
			frontiers.erase(frontier)
		update_portals()
		return

	var placed = _try_place_on_edge(frontier, next_dir, candidate_tile, validation.valid_pieces, validation.dir_to_connect)
	if not placed:
		push_warning("frontier=%s dir=%s tile=%s no fitting piece -> removing edge" % [frontier, next_dir, candidate_tile])
		# guardar punto de spawn definitivo antes de quitar el edge
		finalize_spawn_pos(frontier, next_dir)
		frontier.edges.erase(next_dir)
		if frontier.edges.size() == 0:
			frontiers.erase(frontier)
		update_portals()
		return

	# Si se colocó, poda los edges bloqueados de todas las frontiers
	_prune_all_frontiers_after_placement()
	return


# -------------------- Helper functions (refactor de attach_next_piece) --------------------

# Selecciona una frontier al azar y la devuelve (o null si no hay ninguna).
func _select_frontier() -> MapPiece:
	if frontiers.size() == 0:
		return null
	var fi: int = randi() % frontiers.size()
	return frontiers[fi]


# Devuelve un edge aleatorio de la frontier (asume que frontier tiene al menos uno).
func _pick_random_edge(frontier: MapPiece) -> MapPiece.Dir:
	var edge_list: Array = frontier.edges.duplicate()
	return edge_list[randi() % edge_list.size()]


# Valida un edge y su tile candidato.
# Retorna un Dictionary con campos:
#  - valid: bool
#  - reason: String de diagnóstico (si no válido)
#  - invalid_edges: Array de direcciones inválidas
#  - valid_pieces: Array de MapPieceData que pueden colocarse
#  - dir_to_connect: dirección opuesta esperada en la nueva pieza
func _validate_edge(frontier: MapPiece, next_dir: MapPiece.Dir, candidate_tile: Vector2i) -> Dictionary:
	var result: Dictionary = {}
	result.valid = false
	result.reason = ""
	result.invalid_edges = []
	result.valid_pieces = []
	result.dir_to_connect = get_dir_to_connect(next_dir)

	if grid.has(candidate_tile):
		result.reason = "frontier=%s dir=%s tile=%s reason=occupied" % [frontier, next_dir, candidate_tile]
		return result
	if _would_cause_enclosure_at(candidate_tile):
		result.reason = "frontier=%s dir=%s tile=%s reason=enclose" % [frontier, next_dir, candidate_tile]
		return result

	var invalid_edges = get_invalid_edges_at(candidate_tile, result.dir_to_connect)
	var valid_pieces = map_pieces.filter(func(p: MapPieceData):
		return p.edges.has(result.dir_to_connect) and not invalid_edges.any(func(e): return p.edges.has(e))
	)

	if valid_pieces.size() == 0:
		result.reason = "frontier=%s dir=%s tile=%s reason=invalid_edges %s" % [frontier, next_dir, candidate_tile, invalid_edges]
		result.invalid_edges = invalid_edges
		return result

	result.valid = true
	result.invalid_edges = invalid_edges
	result.valid_pieces = valid_pieces
	return result


# Intenta colocar una pieza en el edge dado. Devuelve true si se colocó correctamente.
func _try_place_on_edge(frontier: MapPiece, next_dir: MapPiece.Dir, candidate_tile: Vector2i, valid_pieces: Array, dir_to_connect: MapPiece.Dir) -> bool:
	var piece_indices: Array = []
	for i in range(valid_pieces.size()):
		piece_indices.append(i)
	piece_indices.shuffle()

	for pi in piece_indices:
		var piece_data: MapPieceData = valid_pieces[pi]
		var new_piece: MapPiece = piece_data.get_instance()
		add_child(new_piece)

		# simular ocupación incluyendo candidate_tile
		var occ_sim: Dictionary = {}
		for k in grid.keys():
			occ_sim[_vec_key(k)] = true
		occ_sim[_vec_key(candidate_tile)] = true

		# comprobar si la nueva pieza deja al menos un camino abierto
		var remaining_dirs: Array = new_piece.edges.duplicate()
		if remaining_dirs.has(dir_to_connect):
			remaining_dirs.erase(dir_to_connect)

		var has_open_path = false
		for rd in remaining_dirs:
			var neigh = candidate_tile + grid_offsets[rd]
			if occ_sim.has(_vec_key(neigh)):
				continue
			if _reachable_to_boundary(neigh, occ_sim):
				has_open_path = true
				break
		if not has_open_path:
			new_piece.queue_free()
			continue

		# commit placement: actualizar datos lógicos y conectar edges
		new_piece.logical_pos = candidate_tile
		grid[candidate_tile] = true
		frontier.set_edge_has_connected(next_dir)
		if new_piece.edges.size() > 0:
			frontiers.append(new_piece)
		if frontier.edges.size() == 0:
			frontiers.erase(frontier)

		attach_piece(frontier, new_piece, next_dir, dir_to_connect)
		new_piece.set_edge_has_connected(dir_to_connect)
		last_piece_attached = new_piece
		# actualizar portales y puntos de spawn tras colocar una pieza
		update_portals()
		return true

	return false


# Poda (elimina) edges bloqueados de todas las frontiers.
func _prune_all_frontiers_after_placement() -> void:
	var remove_frontiers: Array = []
	for f in frontiers:
		var remove_edges: Array = []
		for d in f.edges.duplicate():
			var cand = f.logical_pos + grid_offsets[d]
			if grid.has(cand):
				remove_edges.append(d)
				continue
			if _would_cause_enclosure_at(cand):
				remove_edges.append(d)
				continue
			var inv = get_invalid_edges_at(cand, get_dir_to_connect(d))
			var poss = map_pieces.filter(func(p: MapPieceData): return p.edges.has(get_dir_to_connect(d)) and not inv.any(func(e): return p.edges.has(e)))
			if poss.size() == 0:
				remove_edges.append(d)
				continue
		for re in remove_edges:
			# guardar punto de spawn definitivo antes de quitar el edge
			finalize_spawn_pos(f, re)
			f.edges.erase(re)
		if f.edges.size() == 0:
			remove_frontiers.append(f)
	for rf in remove_frontiers:
		frontiers.erase(rf)

	# actualizar portales tras la poda de frontiers
	update_portals()

func attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece, entry_dir: MapPiece.Dir, exit_dir: MapPiece.Dir) -> void:
	# --- Posicionamiento espacial ---
	var a_world = p_piece_a.get_edge_tile_pos(entry_dir)
	var b_world = p_piece_b.get_edge_tile_pos(exit_dir)
	# compute tile-based offset so edges are adjacent (works for isometric)
	var delta: Vector2i = p_piece_a.get_edge_tile_delta(entry_dir)
	var shift: Vector2 = p_piece_a.get_tile_local_offset(delta)
	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world + shift
	
	# --- Registro de conexiones en el grafo ---
	# Aseguramos que ambas piezas tengan entrada en el diccionario
	if not piece_connections.has(p_piece_a):
		piece_connections[p_piece_a] = {}
	if not piece_connections.has(p_piece_b):
		piece_connections[p_piece_b] = {}
	
	# Conexión bidireccional:
	# - piece_a conecta con piece_b por el borde entry_dir
	# - piece_b conecta con piece_a por el borde exit_dir
	piece_connections[p_piece_a][entry_dir] = p_piece_b
	piece_connections[p_piece_b][exit_dir] = p_piece_a


func finalize_spawn_pos(piece: MapPiece, dir: MapPiece.Dir) -> void:
	# Guarda el punto de spawn definitivo para un edge que ya no crecerá.
	# Incluye la referencia a la pieza para poder reconstruir la ruta hacia el target.
	var tile: Vector2i = piece.logical_pos + grid_offsets[dir]
	var pos: Vector2 = piece.global_position + piece.get_edge_tile_pos(dir) + PORTAL_OFFSET
	# avoid duplicates by logical tile
	for e in finalized_portal_entries:
		if e.has("tile") and e["tile"] == tile:
			return
	finalized_portal_entries.append({
		"tile": tile,
		"pos": pos,
		"dir": dir,
		"piece": piece  # Referencia a la pieza para reconstruir la ruta
	})


func update_portals() -> void:
	# rebuild entries from finalized ones and current frontiers
	portal_entries.clear()
	for e in finalized_portal_entries:
		portal_entries.append(e)

	for f in frontiers:
		for d in f.edges:
			var tile: Vector2i = f.logical_pos + grid_offsets[d]
			var pos: Vector2 = f.global_position + f.get_edge_tile_pos(d) + PORTAL_OFFSET
			portal_entries.append({"tile": tile, "pos": pos, "dir": d, "piece": f})

	# update simple positions list for external use
	portal_spawn_positions.clear()
	for e in portal_entries:
		portal_spawn_positions.append(e["pos"])

	# delegate visual handling to SpawnPositionsHandler
	if spawn_handler != null:
		spawn_handler.update(portal_entries)
		portal_spawn_positions = spawn_handler.get_positions()

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

func get_opostite_dir(dir: MapPiece.Dir) -> MapPiece.Dir:
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
	current_tile += grid_offsets[next_dir]
	return current_tile

func get_invalid_edges(dir_to_connect: MapPiece.Dir) -> Array[MapPiece.Dir]:

	var dirs_check = all_dirs.filter(func(d): return d != dir_to_connect)
	var invalid_dirs: Array[MapPiece.Dir] = []

	# check if any of the adjacent tiles in other directions are occupied,
	# which would block placement of pieces with those edges
	for dir in dirs_check:
		var new_tile := current_tile + grid_offsets[dir]

		if grid.has(new_tile):
			invalid_dirs.append(dir)
			continue

		var simulated := grid.duplicate()
		simulated[new_tile] = true

		if not FloodFill.can_escape_from(new_tile, simulated, grid_offsets):
			invalid_dirs.append(dir)

	return invalid_dirs

func _vec_key(v: Vector2i) -> String:
	return "%d,%d" % [v.x, v.y]

func get_invalid_edges_at(tile: Vector2i, dir_to_connect: MapPiece.Dir) -> Array[MapPiece.Dir]:
	var dirs_check = all_dirs.filter(func(d): return d != dir_to_connect)
	var invalid_dirs: Array[MapPiece.Dir] = []

	for dir in dirs_check:
		var new_tile := tile + grid_offsets[dir]

		if grid.has(new_tile):
			invalid_dirs.append(dir)
			continue

		var simulated := grid.duplicate()
		simulated[new_tile] = true

		if not FloodFill.can_escape_from(new_tile, simulated, grid_offsets):
			invalid_dirs.append(dir)

	# diagnostic
	return invalid_dirs


func _would_cause_enclosure_at(candidate: Vector2i) -> bool:
	# simulate placing at candidate and check if any neighboring empty tile can escape
	var simulated := grid.duplicate()
	simulated[candidate] = true

	for d in all_dirs:
		var n = candidate + grid_offsets[d]
		if simulated.has(n):
			continue
		if FloodFill.can_escape_from(n, simulated, grid_offsets):
			return false
	# no neighbor can escape -> enclosure
	return true


func _frontier_has_valid_edges(piece: MapPiece) -> bool:
	for d in piece.edges:
		var cand = piece.logical_pos + grid_offsets[d]
		if grid.has(cand):
			continue
		if _would_cause_enclosure_at(cand):
			continue
		var invalid = get_invalid_edges_at(cand, get_dir_to_connect(d))
		var possible = map_pieces.filter(func(p: MapPieceData): return p.edges.has(get_dir_to_connect(d)) and not invalid.any(func(e): return p.edges.has(e)))
		if possible.size() > 0:
			return true
	return false


func _reachable_to_boundary(start: Vector2i, occ: Dictionary, lookahead: int = 8) -> bool:
	# occ is a dictionary keyed by string "x,y" representing occupied logical tiles
	var xs: Array = []
	var ys: Array = []
	for k_str in occ.keys():
		var parts = k_str.split(",")
		xs.append(int(parts[0]))
		ys.append(int(parts[1]))

	if xs.size() == 0:
		return true

	var min_x = xs.min()
	var max_x = xs.max()
	var min_y = ys.min()
	var max_y = ys.max()

	min_x -= lookahead
	min_y -= lookahead
	max_x += lookahead
	max_y += lookahead

	var q: Array = []
	var seen: Dictionary = {}
	q.append(start)
	seen[_vec_key(start)] = true

	var neighs: Array = [grid_offsets[MapPiece.Dir.NE], grid_offsets[MapPiece.Dir.SE], grid_offsets[MapPiece.Dir.SW], grid_offsets[MapPiece.Dir.NW]]

	while q.size() > 0:
		var cur: Vector2i = q.pop_front()
		if cur.x <= min_x or cur.x >= max_x or cur.y <= min_y or cur.y >= max_y:
			return true
		for d in neighs:
			var n = cur + d
			var key = _vec_key(n)
			if seen.has(key):
				continue
			if occ.has(key):
				continue
			seen[key] = true
			q.append(n)

	return false


# =============================================================================
# SISTEMA DE RUTAS (waypoints)
# =============================================================================

## Construye la ruta de piezas desde un spawn point hasta la pieza inicial (target).
## Retorna un Array[MapPiece] ordenado: [spawn_piece, ..., init_piece]
## Si no encuentra ruta, retorna array vacío.
func build_route_to_target(spawn_entry: Dictionary) -> Array[MapPiece]:
	if not spawn_entry.has("piece"):
		push_error("[WorldMap] spawn_entry no tiene 'piece'")
		return []
	
	var start_piece: MapPiece = spawn_entry["piece"]
	if start_piece == null or not is_instance_valid(start_piece):
		push_error("[WorldMap] spawn_entry.piece no es válido")
		return []
	
	# Caso trivial: el spawn está en la pieza inicial
	if start_piece == init_piece:
		return [init_piece]
	
	# BFS para encontrar el camino desde start_piece hasta init_piece
	return _find_path_bfs(start_piece, init_piece)


## BFS en el grafo de conexiones para encontrar el camino entre dos piezas.
## Retorna Array[MapPiece] ordenado desde 'from_piece' hasta 'to_piece'.
## Si no hay camino, retorna array vacío.
func _find_path_bfs(from_piece: MapPiece, to_piece: MapPiece) -> Array[MapPiece]:
	if from_piece == to_piece:
		return [from_piece]
	
	# Cola de BFS: cada elemento es la pieza actual
	var queue: Array[MapPiece] = [from_piece]
	# Mapa de "came_from" para reconstruir el camino: piece -> piece_anterior
	var came_from: Dictionary = {}
	came_from[from_piece] = null
	
	while queue.size() > 0:
		var current: MapPiece = queue.pop_front()
		
		# Obtener todas las piezas conectadas a current
		if not piece_connections.has(current):
			continue
		
		var connections: Dictionary = piece_connections[current]
		for dir in connections.keys():
			var neighbor: MapPiece = connections[dir]
			if neighbor == null or not is_instance_valid(neighbor):
				continue
			if came_from.has(neighbor):
				continue  # Ya visitada
			
			came_from[neighbor] = current
			
			# ¿Llegamos al destino?
			if neighbor == to_piece:
				return _reconstruct_path(came_from, from_piece, to_piece)
			
			queue.append(neighbor)
	
	# No se encontró camino
	push_warning("[WorldMap] No se encontró ruta desde %s hasta %s" % [from_piece, to_piece])
	return []


## Reconstruye el camino desde came_from map.
## Retorna Array[MapPiece] ordenado desde 'start' hasta 'end'.
func _reconstruct_path(came_from: Dictionary, start: MapPiece, end: MapPiece) -> Array[MapPiece]:
	var path: Array[MapPiece] = []
	var current: MapPiece = end
	
	while current != null:
		path.append(current)
		if came_from.has(current):
			current = came_from[current]
		else:
			break
	
	# El path está en orden inverso (end -> start), lo invertimos
	path.reverse()
	return path


## Genera un array de waypoints (Vector2 global) para una ruta de piezas.
## spawn_entry: el diccionario del spawn point (con "pos", "dir", "piece")
## route: Array[MapPiece] desde la pieza del spawn hasta init_piece
## 
## Retorna Array[Vector2] con los puntos en orden:
## [spawn_pos, entrada_pieza1, centro_pieza1, salida_pieza1, ..., target]
func build_waypoints_from_route(spawn_entry: Dictionary, route: Array[MapPiece]) -> Array[Vector2]:
	var waypoints: Array[Vector2] = []
	
	if route.size() == 0:
		push_warning("[WorldMap] Ruta vacía, no se pueden generar waypoints")
		return waypoints
	
	# 1. Añadir el punto de spawn como primer waypoint
	if spawn_entry.has("pos"):
		waypoints.append(spawn_entry["pos"])
	
	# 2. Para cada pieza de la ruta, generar: entrada, centro, [salida]
	for i in range(route.size()):
		var piece: MapPiece = route[i]
		var entry_dir: MapPiece.Dir = MapPiece.Dir.NE  # default
		var exit_dir: MapPiece.Dir = MapPiece.Dir.NE   # default
		var has_exit: bool = (i < route.size() - 1)
		
		# Determinar dirección de entrada:
		# - Si es la primera pieza, entrada desde spawn_entry["dir"] (invertida, porque spawn está "fuera")
		# - Si no, entrada desde la pieza anterior
		if i == 0:
			# El enemigo entra por el borde donde está el spawn (dir del spawn_entry)
			if spawn_entry.has("dir"):
				entry_dir = spawn_entry["dir"]
		else:
			# Buscar qué dirección de la pieza anterior conecta con esta
			var prev_piece: MapPiece = route[i - 1]
			entry_dir = _find_connection_dir(prev_piece, piece)
			# La entrada de piece es el lado opuesto
			entry_dir = get_opostite_dir(entry_dir)
		
		# Determinar dirección de salida (si hay siguiente pieza)
		if has_exit:
			var next_piece: MapPiece = route[i + 1]
			exit_dir = _find_connection_dir(piece, next_piece)
		
		# Generar waypoints para esta pieza
		# Entrada: posición global del borde de entrada
		var entry_local: Vector2 = piece.get_edge_tile_pos(entry_dir)
		var entry_global: Vector2 = piece.global_position + entry_local
		waypoints.append(entry_global)
		
		# Centro: posición global de la pieza
		waypoints.append(piece.global_position)
		
		# Salida: solo si hay siguiente pieza
		if has_exit:
			var exit_local: Vector2 = piece.get_edge_tile_pos(exit_dir)
			var exit_global: Vector2 = piece.global_position + exit_local
			waypoints.append(exit_global)
	
	# 3. Añadir el target final (puede ser un portal específico o el centro de init_piece)
	if init_piece != null:
		waypoints.append(init_piece.get_target())
	
	return waypoints


## Encuentra la dirección en 'from_piece' que conecta con 'to_piece'.
## Retorna la dirección o NE como fallback.
func _find_connection_dir(from_piece: MapPiece, to_piece: MapPiece) -> MapPiece.Dir:
	if not piece_connections.has(from_piece):
		push_warning("[WorldMap] from_piece no tiene conexiones registradas")
		return MapPiece.Dir.NE
	
	var connections: Dictionary = piece_connections[from_piece]
	for dir in connections.keys():
		if connections[dir] == to_piece:
			return dir
	
	push_warning("[WorldMap] No se encontró conexión de %s a %s" % [from_piece, to_piece])
	return MapPiece.Dir.NE


## Función de conveniencia: dado un spawn_entry, retorna los waypoints completos.
func get_waypoints_for_spawn(spawn_entry: Dictionary) -> Array[Vector2]:
	var route: Array[MapPiece] = build_route_to_target(spawn_entry)
	return build_waypoints_from_route(spawn_entry, route)


# =============================================================================
# TEST: Spawn de enemigo con waypoints
# =============================================================================

## TEST: Posiciona el enemy exportado en un spawn aleatorio y le asigna waypoints.
## Llamar con "ui_accept" (Enter/Space).
func _test_spawn_enemy_with_waypoints() -> void:
	if enemy == null:
		push_warning("[WorldMap][TEST] No hay enemy asignado en el export")
		return
	
	if portal_entries.size() == 0:
		push_warning("[WorldMap][TEST] No hay spawn points disponibles")
		return
	
	# Elegir un spawn aleatorio
	var spawn_entry: Dictionary = portal_entries[randi() % portal_entries.size()]
	
	# Generar waypoints para esa ruta
	var waypoints: Array[Vector2] = get_waypoints_for_spawn(spawn_entry)
	
	if waypoints.size() == 0:
		push_warning("[WorldMap][TEST] No se pudieron generar waypoints")
		return
	
	# Posicionar el enemy en el primer waypoint (spawn)
	enemy.global_position = waypoints[0]
	enemy.visible = true
	
	# Asignar los waypoints
	enemy.set_waypoints(waypoints)
	
	print("[WorldMap][TEST] Enemy spawned at ", waypoints[0], " with ", waypoints.size(), " waypoints")