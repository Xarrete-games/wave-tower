class_name FrontierManager
extends RefCounted

## Manages frontiers (pieces with available edges to expand) and their pruning.

signal edge_finalized(piece: MapPiece, dir: MapPiece.Dir)

var frontiers: Array[MapPiece] = []
var grid_manager: GridManager
var available_pieces: Array[MapPieceData] = []


func _init(p_grid_manager: GridManager, p_available_pieces: Array[MapPieceData]) -> void:
	grid_manager = p_grid_manager
	available_pieces = p_available_pieces
	frontiers = []

func add_frontier(piece: MapPiece) -> void:
	if piece.edges.size() > 0 and not frontiers.has(piece):
		frontiers.append(piece)


func remove_frontier(piece: MapPiece) -> void:
	frontiers.erase(piece)


func has_frontiers() -> bool:
	return frontiers.size() > 0

## Selects a random frontier.
func select_random_frontier() -> MapPiece:
	if frontiers.size() == 0:
		return null
	return frontiers[randi() % frontiers.size()]


## Selects a random edge from a frontier.
static func pick_random_edge(frontier: MapPiece) -> MapPiece.Dir:
	var edge_list: Array = frontier.edges.duplicate()
	return edge_list[randi() % edge_list.size()]


## Validates an edge and its candidate tile.
## Returns Dictionary with: valid, reason, invalid_edges, valid_pieces, dir_to_connect
func validate_edge(frontier: MapPiece, next_dir: MapPiece.Dir, candidate_tile: Vector2i) -> Dictionary:
	var result: Dictionary = {
		"valid": false,
		"reason": "",
		"invalid_edges": [],
		"valid_pieces": [],
		"dir_to_connect": GridManager.get_opposite_dir(next_dir)
	}

	if grid_manager.is_occupied(candidate_tile):
		result.reason = "frontier=%s dir=%s tile=%s reason=occupied" % [frontier, next_dir, candidate_tile]
		return result
	
	if grid_manager.would_cause_enclosure_at(candidate_tile):
		result.reason = "frontier=%s dir=%s tile=%s reason=enclose" % [frontier, next_dir, candidate_tile]
		return result

	var invalid_edges = grid_manager.get_invalid_edges_at(candidate_tile, result.dir_to_connect)
	var valid_pieces = available_pieces.filter(func(p: MapPieceData):
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


## Removes an edge from a frontier and emits signal when finalized.
func remove_edge_from_frontier(frontier: MapPiece, dir: MapPiece.Dir) -> void:
	edge_finalized.emit(frontier, dir)
	frontier.edges.erase(dir)
	if frontier.edges.size() == 0:
		frontiers.erase(frontier)


## Updates frontiers after placing a piece.
func update_after_placement(old_frontier: MapPiece, new_piece: MapPiece) -> void:
	# Connection edges must already be removed before calling this
	if new_piece.edges.size() > 0:
		frontiers.append(new_piece)
	if old_frontier.edges.size() == 0:
		frontiers.erase(old_frontier)


## Prunes blocked edges from all frontiers.
func prune_all_frontiers() -> void:
	var remove_frontiers: Array = []
	
	for f in frontiers:
		var remove_edges: Array = []
		for d in f.edges.duplicate():
			var cand = grid_manager.get_neighbor_tile(f.logical_pos, d)
			
			if grid_manager.is_occupied(cand):
				remove_edges.append(d)
				continue
			
			if grid_manager.would_cause_enclosure_at(cand):
				remove_edges.append(d)
				continue
			
			var dir_to_connect = GridManager.get_opposite_dir(d)
			var inv = grid_manager.get_invalid_edges_at(cand, dir_to_connect)
			var poss = available_pieces.filter(func(p: MapPieceData): 
				return p.edges.has(dir_to_connect) and not inv.any(func(e): return p.edges.has(e))
			)
			if poss.size() == 0:
				remove_edges.append(d)
				continue
		
		for re in remove_edges:
			edge_finalized.emit(f, re)
			f.edges.erase(re)
		
		if f.edges.size() == 0:
			remove_frontiers.append(f)
	
	for rf in remove_frontiers:
		frontiers.erase(rf)


## Checks if a piece has valid edges to expand.
func frontier_has_valid_edges(piece: MapPiece) -> bool:
	for d in piece.edges:
		var cand = grid_manager.get_neighbor_tile(piece.logical_pos, d)
		
		if grid_manager.is_occupied(cand):
			continue
		if grid_manager.would_cause_enclosure_at(cand):
			continue
		
		var dir_to_connect = GridManager.get_opposite_dir(d)
		var invalid = grid_manager.get_invalid_edges_at(cand, dir_to_connect)
		var possible = available_pieces.filter(func(p: MapPieceData): 
			return p.edges.has(dir_to_connect) and not invalid.any(func(e): return p.edges.has(e))
		)
		if possible.size() > 0:
			return true
	return false


func get_all_frontiers() -> Array[MapPiece]:
	return frontiers.duplicate()
