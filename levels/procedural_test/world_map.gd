class_name WorldMap extends Node2D

const PORTAL_OFFSET: Vector2 = Vector2(0, -80)

@export var init_map_piece_data: MapPieceData
@export var enemy: EnemyProcedural
@export var visual: Node2D

# Managers (inyectados/creados en _ready)
var grid_manager: GridManager = null
var connection_graph: PieceConnectionGraph = null
var frontier_manager: FrontierManager = null
var route_builder: RouteBuilder = null
var spawn_handler: SpawnPositionsHandler = null

# Data
var map_pieces: Array[MapPieceData] = []
var last_piece_attached: MapPiece = null
var init_piece: MapPiece = null

# Portal entries
var portal_entries: Array[Dictionary] = []
var finalized_portal_entries: Array[Dictionary] = []
var portal_spawn_positions: Array[Vector2] = []


func _ready() -> void:
	# Cargar datos
	map_pieces = DataLoader.get_all_map_pieces()
	
	# Crear managers
	grid_manager = GridManager.new()
	connection_graph = PieceConnectionGraph.new()
	frontier_manager = FrontierManager.new(grid_manager, map_pieces)
	spawn_handler = SpawnPositionsHandler.new(visual)
	
	# Conectar señal de frontier_manager para finalizar spawn positions
	frontier_manager.edge_finalized.connect(_on_edge_finalized)
	
	# Instanciar la pieza inicial
	init_piece = init_map_piece_data.get_instance()
	add_child(init_piece)
	init_piece.logical_pos = Vector2i.ZERO
	
	# Registrar en managers
	grid_manager.occupy(Vector2i.ZERO)
	connection_graph.register_piece(init_piece)
	frontier_manager.add_frontier(init_piece)
	
	# Crear route_builder después de tener init_piece
	route_builder = RouteBuilder.new(connection_graph, init_piece)
	
	last_piece_attached = init_piece
	update_portals()


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("test"):
		attach_next_piece()
	if event.is_action_pressed("ui_accept"):
		_test_spawn_enemy_with_waypoints()

func attach_next_piece() -> void:
	if not frontier_manager.has_frontiers():
		push_error("No frontiers available for placement")
		return

	var frontier: MapPiece = frontier_manager.select_random_frontier()
	if frontier == null:
		push_error("Failed to select frontier")
		return

	if frontier.edges.size() == 0:
		frontier_manager.remove_frontier(frontier)
		update_portals()
		return

	var next_dir = FrontierManager.pick_random_edge(frontier)
	var candidate_tile = grid_manager.get_neighbor_tile(frontier.logical_pos, next_dir)

	var validation = frontier_manager.validate_edge(frontier, next_dir, candidate_tile)
	if not validation.valid:
		push_warning(validation.reason)
		frontier_manager.remove_edge_from_frontier(frontier, next_dir)
		update_portals()
		return

	var placed = _try_place_on_edge(frontier, next_dir, candidate_tile, validation.valid_pieces, validation.dir_to_connect)
	if not placed:
		push_warning("frontier=%s dir=%s tile=%s no fitting piece -> removing edge" % [frontier, next_dir, candidate_tile])
		frontier_manager.remove_edge_from_frontier(frontier, next_dir)
		update_portals()
		return

	frontier_manager.prune_all_frontiers()
	update_portals()


func _on_edge_finalized(piece: MapPiece, dir: MapPiece.Dir) -> void:
	_finalize_spawn_pos(piece, dir)


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
		var occ_sim: Dictionary = grid_manager.create_simulated_occupation(candidate_tile)

		# comprobar si la nueva pieza deja al menos un camino abierto
		var remaining_dirs: Array = new_piece.edges.duplicate()
		if remaining_dirs.has(dir_to_connect):
			remaining_dirs.erase(dir_to_connect)

		var has_open_path = false
		for rd in remaining_dirs:
			var neigh = candidate_tile + grid_manager.get_offset(rd)
			if occ_sim.has(GridManager.vec_key(neigh)):
				continue
			if grid_manager.reachable_to_boundary(neigh, occ_sim):
				has_open_path = true
				break
		if not has_open_path:
			new_piece.queue_free()
			continue

		# commit placement
		new_piece.logical_pos = candidate_tile
		grid_manager.occupy(candidate_tile)
		
		# Eliminar edges de conexión ANTES de modificar frontiers
		frontier.set_edge_has_connected(next_dir)
		new_piece.set_edge_has_connected(dir_to_connect)
		
		# Actualizar frontiers
		frontier_manager.update_after_placement(frontier, new_piece)

		_attach_piece(frontier, new_piece, next_dir, dir_to_connect)
		last_piece_attached = new_piece
		return true

	return false


func _attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece, entry_dir: MapPiece.Dir, exit_dir: MapPiece.Dir) -> void:
	# Posicionamiento espacial
	var a_world = p_piece_a.get_edge_tile_pos(entry_dir)
	var b_world = p_piece_b.get_edge_tile_pos(exit_dir)
	var delta: Vector2i = p_piece_a.get_edge_tile_delta(entry_dir)
	var shift: Vector2 = p_piece_a.get_tile_local_offset(delta)
	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world + shift
	
	# Registrar conexión en el grafo
	connection_graph.connect_pieces(p_piece_a, p_piece_b, entry_dir, exit_dir)


func _finalize_spawn_pos(piece: MapPiece, dir: MapPiece.Dir) -> void:
	var tile: Vector2i = grid_manager.get_neighbor_tile(piece.logical_pos, dir)
	var pos: Vector2 = piece.global_position + piece.get_edge_tile_pos(dir) + PORTAL_OFFSET
	var key: String = "%d,%d_%d" % [piece.logical_pos.x, piece.logical_pos.y, dir]
	
	# Evitar duplicados
	for e in finalized_portal_entries:
		if e.has("key") and e["key"] == key:
			return
	
	finalized_portal_entries.append({
		"key": key,
		"tile": tile,
		"pos": pos,
		"dir": dir,
		"piece": piece
	})


func update_portals() -> void:
	portal_entries.clear()
	
	# Añadir entries finalizados
	for e in finalized_portal_entries:
		portal_entries.append(e)

	# Añadir entries de frontiers actuales
	for f in frontier_manager.get_all_frontiers():
		for d in f.edges:
			var tile: Vector2i = grid_manager.get_neighbor_tile(f.logical_pos, d)
			var pos: Vector2 = f.global_position + f.get_edge_tile_pos(d) + PORTAL_OFFSET
			var key: String = "%d,%d_%d" % [f.logical_pos.x, f.logical_pos.y, d]
			portal_entries.append({"key": key, "tile": tile, "pos": pos, "dir": d, "piece": f})
	
	# Actualizar lista de posiciones
	portal_spawn_positions.clear()
	for e in portal_entries:
		portal_spawn_positions.append(e["pos"])

	# Delegar visuales al handler
	if spawn_handler != null:
		spawn_handler.update(portal_entries)
		portal_spawn_positions = spawn_handler.get_positions()


# =============================================================================
# SISTEMA DE RUTAS (waypoints) - delega a RouteBuilder
# =============================================================================

func get_waypoints_for_spawn(spawn_entry: Dictionary) -> Array[Vector2]:
	return route_builder.get_waypoints_for_spawn(spawn_entry)


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