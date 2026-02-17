class_name MapPiece extends Node2D

const size: Vector2i = Vector2i(15, 15)

# Maps Edge.Dir enum to string for path naming
const DIR_NAMES: Dictionary = {
	Edge.Dir.NE: "NE",
	Edge.Dir.SE: "SE",
	Edge.Dir.SW: "SW",
	Edge.Dir.NW: "NW"
}

var edges: Array[Edge] = []
var logical_pos: Vector2i = Vector2i.ZERO

# Cache for snapped route waypoints: { "route_NE_SW": Array[Vector2] }
var _route_cache: Dictionary[String, Array] = {}


@onready var tile_map: TileMapLayer = $MapPieceTileMap


func _ready() -> void:
	_precalculate_routes()


# Get the tile position of the edge in the given direction and position
func get_edge_tile_pos(dir: Edge.Dir, pos: Edge.DirPos = Edge.DirPos.MIDDLE) -> Vector2:
	var used: Array[Vector2i] = tile_map.get_used_cells()
	if used.size() == 0:
		push_error("TileMap has no used cells")
		return Vector2.ZERO

	var edge: Array[Vector2i] = []

	match dir:
		Edge.Dir.NE:
			var min_y = used.map(func(c): return c.y).min()
			edge = used.filter(func(c): return c.y == min_y)
			edge.sort_custom(func(a, b): return a.x < b.x)

		Edge.Dir.SW:
			var max_y = used.map(func(c): return c.y).max()
			edge = used.filter(func(c): return c.y == max_y)
			edge.sort_custom(func(a, b): return a.x < b.x)

		Edge.Dir.NW:
			var min_x = used.map(func(c): return c.x).min()
			edge = used.filter(func(c): return c.x == min_x)
			edge.sort_custom(func(a, b): return a.y < b.y)

		Edge.Dir.SE:
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
		Edge.DirPos.TOP:
			tile_index = int(edge_size * 0.25)
		Edge.DirPos.MIDDLE:
			tile_index = int(edge_size / 2.0)
		Edge.DirPos.BOTTOM:
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
func find_edge_by_dir(dir: Edge.Dir) -> Edge:
	for e in edges:
		if e.dir == dir:
			return e
	return null


## Checks if piece has an edge with given direction
func has_edge_dir(dir: Edge.Dir) -> bool:
	return find_edge_by_dir(dir) != null

func _map_to_local(tile_pos: Vector2i) -> Vector2:
	var local_in_tilemap: Vector2 = tile_map.map_to_local(tile_pos)
	var global_point: Vector2 = tile_map.to_global(local_in_tilemap)
	return to_local(global_point)

func get_edge_normal(dir: Edge.Dir) -> Vector2:
	match dir:
		Edge.Dir.NE:
			return Vector2(0, -1)
		Edge.Dir.SE:
			return Vector2(1, 0)
		Edge.Dir.SW:
			return Vector2(0, 1)
		Edge.Dir.NW:
			return Vector2(-1, 0)
		_:
			return Vector2.ZERO

func get_edge_tile_delta(dir: Edge.Dir) -> Vector2i:
	match dir:
		Edge.Dir.NE:
			return Vector2i(0, -1)
		Edge.Dir.SE:
			return Vector2i(1, 0)
		Edge.Dir.SW:
			return Vector2i(0, 1)
		Edge.Dir.NW:
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
func get_route_waypoints(entry_dir: Edge.Dir, exit_dir: Edge.Dir) -> Array[Vector2]:
	var cache_key: String = _get_route_cache_key(entry_dir, exit_dir)
	
	if _route_cache.has(cache_key):
		var cached: Array = _route_cache[cache_key]
		# Check if we need to reverse based on entry direction
		var canonical_first: Edge.Dir = mini(entry_dir, exit_dir) as Edge.Dir
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
		
		_route_cache[path2d.name] = snapped_points


## Snaps a local position to the center of the nearest tile.
func _snap_to_tile_center(local_pos: Vector2) -> Vector2:
	# Convert local position to tilemap local
	var tilemap_local: Vector2 = tile_map.to_local(to_global(local_pos))
	# Get tile coordinate
	var tile_coord: Vector2i = tile_map.local_to_map(tilemap_local)
	# Convert back to local position (center of tile)
	return _map_to_local(tile_coord)


## Gets the cache key for a route (always uses canonical order)
func _get_route_cache_key(dir_a: Edge.Dir, dir_b: Edge.Dir) -> String:
	var first: Edge.Dir = mini(dir_a, dir_b) as Edge.Dir
	var second: Edge.Dir = maxi(dir_a, dir_b) as Edge.Dir
	return "route_%s_%s" % [DIR_NAMES[first], DIR_NAMES[second]]


## Gets waypoints for the final route (last piece to end).
## Path naming: "route_{DIR}_END"
func get_final_route_waypoints(entry_dir: Edge.Dir) -> Array[Vector2]:
	var cache_key: String = "route_%s_END" % DIR_NAMES[entry_dir]
	
	if _route_cache.has(cache_key):
		var cached: Array = _route_cache[cache_key]
		var result: Array[Vector2] = []
		for pt in cached:
			result.append(pt)
		return result
	
	return []
