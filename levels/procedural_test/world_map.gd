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
# entries keep both position and direction so we can flip sprites correctly
var portal_entries: Array = []
var finalized_portal_entries: Array = []


func _ready() -> void:
	var init_piece: MapPiece = init_map_piece_data.get_instance()
	map_pieces = DataLoader.get_all_map_pieces()
	grid[Vector2i.ZERO] = true
	current_tile = Vector2i.ZERO
	add_child(init_piece)
	# logical coord for initial piece
	init_piece.logical_pos = Vector2i.ZERO
	if init_piece.edges.size() > 0:
		frontiers.append(init_piece)

	last_piece_attached = init_piece

	# crear portales para los edges actuales
	update_portals()

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("test"):
		attach_next_piece()

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
	var a_world = p_piece_a.get_edge_tile_pos(entry_dir)
	var b_world = p_piece_b.get_edge_tile_pos(exit_dir)
	# compute tile-based offset so edges are adjacent (works for isometric)
	var delta: Vector2i = p_piece_a.get_edge_tile_delta(entry_dir)
	var shift: Vector2 = p_piece_a.get_tile_local_offset(delta)

	p_piece_b.global_position = p_piece_a.global_position + a_world - b_world + shift


func finalize_spawn_pos(piece: MapPiece, dir: MapPiece.Dir) -> void:
	# compute global spawn position for the given edge and add to finalized list
	var pos: Vector2 = piece.global_position + piece.get_edge_tile_pos(dir) + PORTAL_OFFSET
	# avoid duplicates by position
	for e in finalized_portal_entries:
		if e["pos"].distance_to(pos) < 1e-3:
			return
	finalized_portal_entries.append({"pos": pos, "dir": dir})


func update_portals() -> void:
	# remove existing portal instances
	for p in get_tree().get_nodes_in_group("orange_portal"):
		if is_instance_valid(p):
			p.queue_free()
	# rebuild entries from finalized ones and current frontiers
	portal_entries = finalized_portal_entries.duplicate()

	for f in frontiers:
		for d in f.edges:
			var pos: Vector2 = f.global_position + f.get_edge_tile_pos(d) + PORTAL_OFFSET 
			portal_entries.append({"pos": pos, "dir": d})

	# update simple positions list for external use
	portal_spawn_positions.clear()
	for e in portal_entries:
		portal_spawn_positions.append(e["pos"])

	# instantiate portals for all entries
	for e in portal_entries:
		var portal = ORANGE_PORTAL.instantiate()
		visual.add_child(portal)
		portal.global_position = e["pos"]
		portal.add_to_group("orange_portal")

		# flip horizontally if the edge points east (NE or SE)
		if e["dir"] == MapPiece.Dir.NE or e["dir"] == MapPiece.Dir.SE:
			var sprite_node = portal.get_node_or_null("AnimatedSprite2D")
			if sprite_node and sprite_node is AnimatedSprite2D:
				sprite_node.flip_h = true
			else:
				for c in portal.get_children():
					if c is AnimatedSprite2D:
						c.flip_h = true
						break

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
