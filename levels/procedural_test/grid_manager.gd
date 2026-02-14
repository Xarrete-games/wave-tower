class_name GridManager
extends RefCounted

## Manages the logical map grid: tile occupation, directions and spatial validation.

const GRID_OFFSETS: Dictionary[Edge.Dir, Vector2i] = {
	Edge.Dir.NE: Vector2i(1, -1),
	Edge.Dir.SE: Vector2i(1, 0),
	Edge.Dir.SW: Vector2i(-1, 1),
	Edge.Dir.NW: Vector2i(-1, 0)
}

const ALL_DIRS: Array[Edge.Dir] = [Edge.Dir.NE, Edge.Dir.SE, Edge.Dir.SW, Edge.Dir.NW]

var grid: Dictionary[Vector2i, bool] = {}

func _init() -> void:
	grid = {}

func occupy(tile: Vector2i) -> void:
	grid[tile] = true

func is_occupied(tile: Vector2i) -> bool:
	return grid.has(tile)

func get_neighbor_tile(tile: Vector2i, dir: Edge.Dir) -> Vector2i:
	return tile + GRID_OFFSETS[dir]

func get_offset(dir: Edge.Dir) -> Vector2i:
	return GRID_OFFSETS[dir]


## Checks if placing a piece at candidate would cause an enclosure.
func would_cause_enclosure_at(candidate: Vector2i) -> bool:
	var simulated := grid.duplicate()
	simulated[candidate] = true

	for d in ALL_DIRS:
		var n = candidate + GRID_OFFSETS[d]
		if simulated.has(n):
			continue
		if FloodFill.can_escape_from(n, simulated, GRID_OFFSETS):
			return false
	return true


## Gets invalid directions for placing a piece at tile.
func get_invalid_edges_at(tile: Vector2i, dir_to_connect: Edge.Dir) -> Array[Edge.Dir]:
	var dirs_check = ALL_DIRS.filter(func(d): return d != dir_to_connect)
	var invalid_dirs: Array[Edge.Dir] = []

	for dir in dirs_check:
		var new_tile := tile + GRID_OFFSETS[dir]

		if grid.has(new_tile):
			invalid_dirs.append(dir)
			continue

		var simulated := grid.duplicate()
		simulated[new_tile] = true

		if not FloodFill.can_escape_from(new_tile, simulated, GRID_OFFSETS):
			invalid_dirs.append(dir)

	return invalid_dirs


## Checks if from start you can reach the map boundary.
func reachable_to_boundary(start: Vector2i, occ: Dictionary, lookahead: int = 8) -> bool:
	var xs: Array = []
	var ys: Array = []
	for k_str in occ.keys():
		var parts = k_str.split(",")
		xs.append(int(parts[0]))
		ys.append(int(parts[1]))

	if xs.size() == 0:
		return true

	var min_x = xs.min() - lookahead
	var max_x = xs.max() + lookahead
	var min_y = ys.min() - lookahead
	var max_y = ys.max() + lookahead

	var q: Array = []
	var seen: Dictionary = {}
	q.append(start)
	seen[vec_key(start)] = true

	var neighs: Array = [GRID_OFFSETS[Edge.Dir.NE], GRID_OFFSETS[Edge.Dir.SE], GRID_OFFSETS[Edge.Dir.SW], GRID_OFFSETS[Edge.Dir.NW]]

	while q.size() > 0:
		var cur: Vector2i = q.pop_front()
		if cur.x <= min_x or cur.x >= max_x or cur.y <= min_y or cur.y >= max_y:
			return true
		for d in neighs:
			var n = cur + d
			var key = vec_key(n)
			if seen.has(key):
				continue
			if occ.has(key):
				continue
			seen[key] = true
			q.append(n)

	return false


## Creates a simulated occupation dictionary (for validations).
func create_simulated_occupation(extra_tile: Vector2i = Vector2i(-99999, -99999)) -> Dictionary:
	var occ: Dictionary = {}
	for k in grid.keys():
		occ[vec_key(k)] = true
	if extra_tile != Vector2i(-99999, -99999):
		occ[vec_key(extra_tile)] = true
	return occ


static func vec_key(v: Vector2i) -> String:
	return "%d,%d" % [v.x, v.y]
