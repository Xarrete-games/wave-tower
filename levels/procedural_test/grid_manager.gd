class_name GridManager
extends RefCounted

## Manages the logical map grid: tile occupation, directions and spatial validation.

const GRID_OFFSETS: Dictionary[MapPiece.Dir, Vector2i] = {
	MapPiece.Dir.NE: Vector2i(1, -1),
	MapPiece.Dir.SE: Vector2i(1, 0),
	MapPiece.Dir.SW: Vector2i(-1, 1),
	MapPiece.Dir.NW: Vector2i(-1, 0)
}

const ALL_DIRS: Array[MapPiece.Dir] = [MapPiece.Dir.NE, MapPiece.Dir.SE, MapPiece.Dir.SW, MapPiece.Dir.NW]

var grid: Dictionary = {}  # Vector2i -> bool


func _init() -> void:
	grid = {}


func occupy(tile: Vector2i) -> void:
	grid[tile] = true


func is_occupied(tile: Vector2i) -> bool:
	return grid.has(tile)


func get_neighbor_tile(tile: Vector2i, dir: MapPiece.Dir) -> Vector2i:
	return tile + GRID_OFFSETS[dir]


func get_offset(dir: MapPiece.Dir) -> Vector2i:
	return GRID_OFFSETS[dir]


## Returns the opposite direction (for connections)
static func get_opposite_dir(dir: MapPiece.Dir) -> MapPiece.Dir:
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
func get_invalid_edges_at(tile: Vector2i, dir_to_connect: MapPiece.Dir) -> Array[MapPiece.Dir]:
	var dirs_check = ALL_DIRS.filter(func(d): return d != dir_to_connect)
	var invalid_dirs: Array[MapPiece.Dir] = []

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

	var neighs: Array = [GRID_OFFSETS[MapPiece.Dir.NE], GRID_OFFSETS[MapPiece.Dir.SE], GRID_OFFSETS[MapPiece.Dir.SW], GRID_OFFSETS[MapPiece.Dir.NW]]

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
