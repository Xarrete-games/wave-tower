class_name FrontierManager
extends RefCounted

## Manages frontiers (pieces with available edges to expand) and their pruning.

signal edge_finalized(piece: MapPiece, edge: Edge)

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
static func pick_random_edge(frontier: MapPiece) -> Edge:
	var edge_list: Array = frontier.edges.duplicate()
	return edge_list[randi() % edge_list.size()]


## Validates an edge and its candidate tile.
## Returns Dictionary with: valid, reason, invalid_edges, valid_pieces, edge_to_connect
func validate_edge(frontier: MapPiece, next_edge: Edge, candidate_tile: Vector2i) -> Dictionary:
	var edge_to_connect: Edge = next_edge.get_opposite()
	var result: Dictionary = {
		"valid": false,
		"reason": "",
		"invalid_edges": [],
		"valid_pieces": [],
		"edge_to_connect": edge_to_connect
	}

	if grid_manager.is_occupied(candidate_tile):
		result.reason = "frontier=%s edge=%s tile=%s reason=occupied" % [frontier, next_edge, candidate_tile]
		return result
	
	if grid_manager.would_cause_enclosure_at(candidate_tile):
		result.reason = "frontier=%s edge=%s tile=%s reason=enclose" % [frontier, next_edge, candidate_tile]
		return result

	var invalid_edges = grid_manager.get_invalid_edges_at(candidate_tile, edge_to_connect.dir)
	# Find pieces that have a matching edge (dir + pos) and don't have blocked edges
	var valid_pieces = available_pieces.filter(func(p: MapPieceData):
		return p.has_connecting_edge(next_edge) and not invalid_edges.any(func(e): return p.has_edge_dir(e))
	)

	if valid_pieces.size() == 0:
		result.reason = "frontier=%s edge=%s tile=%s reason=invalid_edges %s" % [frontier, next_edge, candidate_tile, invalid_edges]
		result.invalid_edges = invalid_edges
		return result

	result.valid = true
	result.invalid_edges = invalid_edges
	result.valid_pieces = valid_pieces
	return result


## Removes an edge from a frontier and emits signal when finalized.
func remove_edge_from_frontier(frontier: MapPiece, edge: Edge) -> void:
	edge_finalized.emit(frontier, edge)
	# Find and erase the matching edge by dir and pos
	for i in range(frontier.edges.size() - 1, -1, -1):
		if frontier.edges[i].matches(edge):
			frontier.edges.remove_at(i)
			break
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
		var remove_edges: Array[Edge] = []
		for edge in f.edges.duplicate():
			var cand = grid_manager.get_neighbor_tile(f.logical_pos, edge.dir)
			
			if grid_manager.is_occupied(cand):
				remove_edges.append(edge)
				continue
			
			if grid_manager.would_cause_enclosure_at(cand):
				remove_edges.append(edge)
				continue
			
			var edge_to_connect = edge.get_opposite()
			var inv = grid_manager.get_invalid_edges_at(cand, edge_to_connect.dir)
			# Find pieces with matching edge (dir + pos) that don't have blocked edges
			var poss = available_pieces.filter(func(p: MapPieceData): 
				return p.has_connecting_edge(edge) and not inv.any(func(e): return p.has_edge_dir(e))
			)
			if poss.size() == 0:
				remove_edges.append(edge)
				continue
		
		for re in remove_edges:
			edge_finalized.emit(f, re)
			# Find and erase the matching edge
			for i in range(f.edges.size() - 1, -1, -1):
				if f.edges[i].matches(re):
					f.edges.remove_at(i)
					break
		
		if f.edges.size() == 0:
			remove_frontiers.append(f)
	
	for rf in remove_frontiers:
		frontiers.erase(rf)


## Checks if a piece has valid edges to expand.
func frontier_has_valid_edges(piece: MapPiece) -> bool:
	for edge in piece.edges:
		var cand = grid_manager.get_neighbor_tile(piece.logical_pos, edge.dir)
		
		if grid_manager.is_occupied(cand):
			continue
		if grid_manager.would_cause_enclosure_at(cand):
			continue
		
		var edge_to_connect = edge.get_opposite()
		var invalid = grid_manager.get_invalid_edges_at(cand, edge_to_connect.dir)
		# Find pieces with matching edge (dir + pos) that don't have blocked edges
		var possible = available_pieces.filter(func(p: MapPieceData): 
			return p.has_connecting_edge(edge) and not invalid.any(func(e): return p.has_edge_dir(e))
		)
		if possible.size() > 0:
			return true
	return false


func get_all_frontiers() -> Array[MapPiece]:
	return frontiers.duplicate()
