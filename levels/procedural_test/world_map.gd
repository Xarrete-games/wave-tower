class_name WorldMap extends Node2D

const PORTAL_OFFSET: Vector2 = Vector2(0, -80)
const WAVES_PER_BOSS: int = 10

@export var visual: Node2D
@export var composite_tile_map: CompositeTileMap

# Managers (injected/created in _ready)
var grid_manager: GridManager = null
var connection_graph: PieceConnectionGraph = null
var frontier_manager: FrontierManager = null
var route_builder: RouteBuilder = null
var spawn_handler: SpawnPositionsHandler = null

# Data
var map_pieces: Array[MapPieceData] = []
var last_piece_attached: MapPiece = null

# Portal entries
var portal_entries: Array[Dictionary] = []
var finalized_portal_entries: Array[Dictionary] = []
var portal_spawn_positions: Array[Vector2] = []

# Fork injection flow:
# after each boss cycle, keep trying to place a fork (with priority)
# until one is successfully placed.
var _pending_fork_after_boss: bool = false


func _ready() -> void:
	# Load data
	map_pieces = DataLoader.get_all_map_pieces()
	
	# Create managers
	grid_manager = GridManager.new()
	connection_graph = PieceConnectionGraph.new()
	frontier_manager = FrontierManager.new(grid_manager, map_pieces)
	spawn_handler = SpawnPositionsHandler.new(visual)
	
	# Connect frontier_manager signal to finalize spawn positions
	frontier_manager.edge_finalized.connect(_on_edge_finalized)
	
	# Instantiate initial piece
	var init_piece = DataLoader.get_all_initial_map_pieces().pick_random().get_instance()
	add_child(init_piece)
	init_piece.logical_pos = Vector2i.ZERO
	_move_piece_decoration_to_visuals(init_piece)
	
	# Register in managers
	grid_manager.occupy(Vector2i.ZERO)
	connection_graph.register_piece(init_piece)
	if composite_tile_map:
		composite_tile_map.register_piece(init_piece)
	frontier_manager.add_frontier(init_piece)
	
	# Create route_builder after having init_piece
	route_builder = RouteBuilder.new(connection_graph, init_piece)
	
	last_piece_attached = init_piece
	update_portals()
	#init with one piece
	attach_next_piece()
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("test"):
		attach_next_piece()

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

	var next_edge: Edge = FrontierManager.pick_random_edge(frontier)
	var candidate_tile = grid_manager.get_neighbor_tile(frontier.logical_pos, next_edge.dir)

	var validation = frontier_manager.validate_edge(frontier, next_edge, candidate_tile)
	if not validation.valid:
		push_warning(validation.reason)
		frontier_manager.remove_edge_from_frontier(frontier, next_edge)
		update_portals()
		return

	var placed = _try_place_on_edge(frontier, next_edge, candidate_tile, validation.valid_pieces, validation.edge_to_connect)
	if not placed:
		push_warning("frontier=%s edge=%s tile=%s no fitting piece -> removing edge" % [frontier, next_edge, candidate_tile])
		frontier_manager.remove_edge_from_frontier(frontier, next_edge)
		update_portals()
		return

	frontier_manager.prune_all_frontiers()
	update_portals()


func _on_edge_finalized(piece: MapPiece, edge: Edge) -> void:
	_finalize_spawn_pos(piece, edge)


func _try_place_on_edge(frontier: MapPiece, next_edge: Edge, candidate_tile: Vector2i, valid_pieces: Array, edge_to_connect: Edge) -> bool:
	var candidate_pieces: Array = []

	if _pending_fork_after_boss:
		var fork_pieces: Array = valid_pieces.filter(func(p: MapPieceData): return p.is_fork)
		var other_pieces: Array = valid_pieces.filter(func(p: MapPieceData): return not p.is_fork)
		fork_pieces.shuffle()
		other_pieces.shuffle()
		candidate_pieces = fork_pieces + other_pieces
	else:
		candidate_pieces = valid_pieces.filter(func(p: MapPieceData): return not p.is_fork)
		candidate_pieces.shuffle()

	for piece_data in candidate_pieces:
		var new_piece: MapPiece = piece_data.get_instance()
		add_child(new_piece)

		# simulate occupation including candidate_tile
		var occ_sim: Dictionary = grid_manager.create_simulated_occupation(candidate_tile)

		# check if new piece leaves at least one open path
		var remaining_edges: Array = new_piece.edges.duplicate()
		# Remove the connecting edge from remaining
		for i in range(remaining_edges.size() - 1, -1, -1):
			if remaining_edges[i].matches(edge_to_connect):
				remaining_edges.remove_at(i)
				break

		var has_open_path = false
		for re in remaining_edges:
			var neigh = candidate_tile + grid_manager.get_offset(re.dir)
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
		if composite_tile_map:
			composite_tile_map.register_piece(new_piece)
		
		# Remove connection edges BEFORE modifying frontiers
		frontier.set_edge_has_connected(next_edge)
		new_piece.set_edge_has_connected(edge_to_connect)
		
		# Update frontiers
		frontier_manager.update_after_placement(frontier, new_piece)

		_attach_piece(frontier, new_piece, next_edge.dir, edge_to_connect.dir)
		_move_piece_decoration_to_visuals(new_piece)
		last_piece_attached = new_piece

		# If we were waiting for a post-boss fork and we placed one, clear pending state.
		if _pending_fork_after_boss and piece_data.is_fork:
			_pending_fork_after_boss = false

		return true

	return false


func _attach_piece(p_piece_a: MapPiece, p_piece_b: MapPiece, entry_dir: Edge.Dir, exit_dir: Edge.Dir) -> void:
	# Spatial positioning
	var a_world = p_piece_a.get_edge_tile_pos(entry_dir)
	var b_world = p_piece_b.get_edge_tile_pos(exit_dir)
	var delta: Vector2i = p_piece_a.get_edge_tile_delta(entry_dir)
	var shift: Vector2 = p_piece_a.get_tile_local_offset(delta)
	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world + shift
	
	# Register connection in graph
	connection_graph.connect_pieces(p_piece_a, p_piece_b, entry_dir, exit_dir)


func _move_piece_decoration_to_visuals(piece: MapPiece) -> void:
	if piece == null:
		return
	if visual == null:
		push_warning("[WorldMap] visual container is null; cannot move decoration for piece %s" % piece.name)
		return

	var decoration: Node2D = piece.get_decoration()
	if decoration == null:
		return

	var deco_children: Array[Node] = decoration.get_children()
	for child in deco_children:
		if not (child is Node2D):
			continue
		if child.get_parent() == visual:
			continue
		child.reparent(visual, true)


func _finalize_spawn_pos(piece: MapPiece, edge: Edge) -> void:
	var tile: Vector2i = grid_manager.get_neighbor_tile(piece.logical_pos, edge.dir)
	var pos: Vector2 = piece.global_position + piece.get_edge_tile_pos(edge.dir, edge.pos) + PORTAL_OFFSET
	var key: String = "%d,%d_%d_%d" % [piece.logical_pos.x, piece.logical_pos.y, edge.dir, edge.pos]
	
	# Avoid duplicates
	for e in finalized_portal_entries:
		if e.has("key") and e["key"] == key:
			return
	
	finalized_portal_entries.append({
		"key": key,
		"tile": tile,
		"pos": pos,
		"edge": edge,
		"piece": piece
	})


func update_portals() -> void:
	portal_entries.clear()
	
	# Add finalized entries
	for e in finalized_portal_entries:
		portal_entries.append(e)

	# Add current frontier entries
	for f in frontier_manager.get_all_frontiers():
		for edge in f.edges:
			var tile: Vector2i = grid_manager.get_neighbor_tile(f.logical_pos, edge.dir)
			var pos: Vector2 = f.global_position + f.get_edge_tile_pos(edge.dir, edge.pos) + PORTAL_OFFSET
			var key: String = "%d,%d_%d_%d" % [f.logical_pos.x, f.logical_pos.y, edge.dir, edge.pos]
			portal_entries.append({"key": key, "tile": tile, "pos": pos, "edge": edge, "piece": f})
	
	# Update positions list
	portal_spawn_positions.clear()
	for e in portal_entries:
		portal_spawn_positions.append(e["pos"])

	# Delegate visuals to handler
	if spawn_handler != null:
		spawn_handler.update(portal_entries)
		portal_spawn_positions = spawn_handler.get_positions()


# =============================================================================
# ROUTE SYSTEM (waypoints) - delegates to RouteBuilder
# =============================================================================

func get_waypoints_for_spawn(spawn_entry: Dictionary) -> Array[Vector2]:
	return route_builder.get_waypoints_for_spawn(spawn_entry)

func _on_wave_finished() -> void:
	var current_wave = RunContext.progress.current_wave

	# Boss cycles are every 10 waves: after each boss, start forcing fork priority
	# until one fork is successfully placed.
	if current_wave % WAVES_PER_BOSS == 0:
		_pending_fork_after_boss = true

	# Normal growth cadence is every 2 waves, but while a post-boss fork is pending,
	# retry placement every wave until a fork enters the map.
	if current_wave % 3 == 0:
		attach_next_piece()
