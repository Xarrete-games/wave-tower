class_name MapPiece extends Node2D

enum Dir { NE, SE, SW, NW }
enum DirPos { TOP, MIDDLE, BOTTOM }

const size: Vector2i = Vector2i(15, 15)
const BUILDEABLE_CUSTOM_DATA: String = "buildeable"
const BLOCKED_CUSTOM_DATA: String = "blocked"
const ATLAS_ID: int = 0
const NORMAL_TILE_POS: Vector2i = Vector2i(2, 0)

# Maps direction enum to string for path naming
const DIR_NAMES: Dictionary = {
	Dir.NE: "NE",
	Dir.SE: "SE",
	Dir.SW: "SW",
	Dir.NW: "NW"
}

var edges: Array[Edge] = []
var logical_pos: Vector2i = Vector2i.ZERO

# Cache for snapped route waypoints: { "route_NE_SW": Array[Array[Vector2]] }
# Each key maps to an array of route variants (1 or more paths)
var _route_cache: Dictionary[String, Array] = {}


@onready var tile_map: TileMapLayer = $MapPieceTileMap
@onready var decoration: Node2D = $Decoration

func _ready() -> void:
	_precalculate_routes()

func get_decoration() -> Node2D:
	return decoration

# Get the tile position of the edge in the given direction and position
func get_edge_tile_pos(dir: int, pos: int = DirPos.MIDDLE) -> Vector2:
	var used: Array[Vector2i] = tile_map.get_used_cells()
	if used.size() == 0:
		push_error("TileMap has no used cells")
		return Vector2.ZERO

	var edge: Array[Vector2i] = []

	match dir:
		Dir.NE:
			var min_y = used.map(func(c): return c.y).min()
			edge = used.filter(func(c): return c.y == min_y)
			edge.sort_custom(func(a, b): return a.x < b.x)

		Dir.SW:
			var max_y = used.map(func(c): return c.y).max()
			edge = used.filter(func(c): return c.y == max_y)
			edge.sort_custom(func(a, b): return a.x < b.x)

		Dir.NW:
			var min_x = used.map(func(c): return c.x).min()
			edge = used.filter(func(c): return c.x == min_x)
			edge.sort_custom(func(a, b): return a.y < b.y)

		Dir.SE:
			var max_x = used.map(func(c): return c.x).max()
			edge = used.filter(func(c): return c.x == max_x)
			edge.sort_custom(func(a, b): return a.y < b.y)

	if edge.size() == 0:
		push_error("Edge has no tiles for dir %s" % [dir])
		return Vector2.ZERO

	# Calculate tile index based on DirPos
	# For 11 tiles: TOP=2 (1/4), MIDDLE=5 (center), BOTTOM=8 (3/4)
	var edge_size: int = edge.size()
	var tile_index: int
	match pos:
		DirPos.TOP:
			tile_index = int(edge_size * 0.25)
		DirPos.MIDDLE:
			tile_index = int(edge_size / 2.0)
		DirPos.BOTTOM:
			tile_index = int(edge_size * 0.75)
		_:
			tile_index = int(edge_size / 2.0)
	
	tile_index = clampi(tile_index, 0, edge_size - 1)
	var tile: Vector2i = edge[tile_index]

	return _map_to_local(tile)

func set_edge_has_connected(edge: Edge) -> void:
	if edges.size() == 0:
		push_error("Trying to set edge %s as connected, but this piece has no edges" % [edge])
		return

	var to_remove: Edge = null
	for e in edges:
		if e.matches(edge):
			to_remove = e
			break
	
	if to_remove == null:
		push_error("Trying to set edge %s as connected, but it is not an edge in this piece" % [edge])
		return

	edges.erase(to_remove)


## Finds an edge by direction (returns first match or null)
func find_edge_by_dir(dir: int) -> Edge:
	for e in edges:
		if e.dir == dir:
			return e
	return null


## Checks if piece has an edge with given direction
func has_edge_dir(dir: int) -> bool:
	return find_edge_by_dir(dir) != null

func _map_to_local(tile_pos: Vector2i) -> Vector2:
	var local_in_tilemap: Vector2 = tile_map.map_to_local(tile_pos)
	var global_point: Vector2 = tile_map.to_global(local_in_tilemap)
	return to_local(global_point)

func get_edge_normal(dir: int) -> Vector2:
	match dir:
		Dir.NE:
			return Vector2(0, -1)
		Dir.SE:
			return Vector2(1, 0)
		Dir.SW:
			return Vector2(0, 1)
		Dir.NW:
			return Vector2(-1, 0)
		_:
			return Vector2.ZERO

func get_edge_tile_delta(dir: int) -> Vector2i:
	match dir:
		Dir.NE:
			return Vector2i(0, -1)
		Dir.SE:
			return Vector2i(1, 0)
		Dir.SW:
			return Vector2i(0, 1)
		Dir.NW:
			return Vector2i(-1, 0)
		_:
			return Vector2i(0, 0)

func get_tile_local_offset(delta: Vector2i) -> Vector2:
	var origin = _map_to_local(Vector2i(0, 0))
	var target = _map_to_local(delta)
	return target - origin


## Gets intermediate waypoints for a route between two edges.
## Returns points in correct order (entry→exit). Does NOT include entry/exit positions.
## Points are snapped to tile centers for precise movement.
func get_route_waypoints(entry_dir: int, exit_dir: int) -> Array[Vector2]:
	var cache_key: String = _get_route_cache_key(entry_dir, exit_dir)
	
	if _route_cache.has(cache_key):
		var variants: Array = _route_cache[cache_key]
		var cached: Array = variants[randi() % variants.size()]
		# Check if we need to reverse based on entry direction
		var canonical_first: int = mini(entry_dir, exit_dir)
		if entry_dir != canonical_first:
			var reversed: Array[Vector2] = []
			for i in range(cached.size() - 1, -1, -1):
				reversed.append(cached[i])
			return reversed
		else:
			var result: Array[Vector2] = []
			for pt in cached:
				result.append(pt)
			return result
	
	return []


## Pre-calculates all route waypoints snapped to tile centers.
func _precalculate_routes() -> void:
	_route_cache.clear()
	
	# Find all Path2D nodes in Routes container or as direct children
	var path_nodes: Array[Path2D] = []
	var routes_container: Node = get_node_or_null("Routes")
	if routes_container:
		for child in routes_container.get_children():
			if child is Path2D and child.name.begins_with("route_"):
				path_nodes.append(child)
	
	# Also check direct children
	for child in get_children():
		if child is Path2D and child.name.begins_with("route_"):
			path_nodes.append(child)
	
	# Process each path
	for path2d in path_nodes:
		var curve: Curve2D = path2d.curve
		if curve == null or curve.point_count == 0:
			continue
		
		var snapped_points: Array[Vector2] = []
		for i in range(curve.point_count):
			var point_local: Vector2 = path2d.position + curve.get_point_position(i)
			var tile_center: Vector2 = _snap_to_tile_center(point_local)
			
			# Avoid duplicate consecutive points
			if snapped_points.size() == 0 or snapped_points[-1] != tile_center:
				snapped_points.append(tile_center)
		
		# Strip variant suffix to get base key (e.g. "route_NE_SW_2" -> "route_NE_SW")
		var base_key: String = _get_route_base_key(path2d.name)
		if not _route_cache.has(base_key):
			_route_cache[base_key] = []
		_route_cache[base_key].append(snapped_points)


## Snaps a local position to the center of the nearest tile.
func _snap_to_tile_center(local_pos: Vector2) -> Vector2:
	# Convert local position to tilemap local
	var tilemap_local: Vector2 = tile_map.to_local(to_global(local_pos))
	# Get tile coordinate
	var tile_coord: Vector2i = tile_map.local_to_map(tilemap_local)
	# Convert back to local position (center of tile)
	return _map_to_local(tile_coord)


## Gets the cache key for a route (always uses canonical order)
func _get_route_cache_key(dir_a: int, dir_b: int) -> String:
	var first: int = mini(dir_a, dir_b)
	var second: int = maxi(dir_a, dir_b)
	return "route_%s_%s" % [DIR_NAMES[first], DIR_NAMES[second]]


## Strips variant suffix from path name to get the base route key.
## e.g. "route_NE_SW_2" -> "route_NE_SW", "route_NE_END_3" -> "route_NE_END"
func _get_route_base_key(path_name: String) -> String:
	var regex = RegEx.new()
	regex.compile("^(route_[A-Z]+_[A-Z]+)(?:_\\d+)?$")
	var result = regex.search(path_name)
	if result:
		return result.get_string(1)
	return path_name


## Reduces buildeable tiles to [param max_count] random ones.
## Excess buildeable tiles are converted to normal (non-buildeable) tiles visually.
## Must be called AFTER the piece is in the tree (tile_map ready).
func limit_buildeable_tiles(max_count: int) -> void:
	var tm: TileMapLayer = tile_map
	var buildeable_coords: Array[Vector2i] = []

	for coords in tm.get_used_cells():
		var td: TileData = tm.get_cell_tile_data(coords)
		if td == null:
			continue
		if td.get_custom_data(BLOCKED_CUSTOM_DATA) == true:
			continue
		if td.get_custom_data(BUILDEABLE_CUSTOM_DATA) == true:
			buildeable_coords.append(coords)

	if buildeable_coords.size() <= max_count:
		return

	# Keep max_count random tiles, convert the rest
	buildeable_coords.shuffle()
	var to_remove: Array[Vector2i] = buildeable_coords.slice(max_count)
	for coords in to_remove:
		tm.set_cell(coords, ATLAS_ID, NORMAL_TILE_POS)


## Gets waypoints for the final route (last piece to end).
## Path naming: "route_{DIR}_END"
func get_final_route_waypoints(entry_dir: int) -> Array[Vector2]:
	var cache_key: String = "route_%s_END" % DIR_NAMES[entry_dir]
	
	if _route_cache.has(cache_key):
		var variants: Array = _route_cache[cache_key]
		var cached: Array = variants[randi() % variants.size()]
		var result: Array[Vector2] = []
		for pt in cached:
			result.append(pt)
		return result
	
	return []
