class_name CompositeTileMap extends Node
## Facade that aggregates multiple MapPiece tilemaps into a single
## logical surface for tower placement.
##
## It exposes the same public interface that TowerPlacer expects
## (is_mouse_on_buildeable_tile, get_current_tile_pos, etc.)
## so the placer does not need to know whether the level is made
## of one tilemap or many.
##
## Each tile is identified internally by a TileKey {piece, coords}
## to avoid collisions between pieces that share the same local coords.

# ---------------------------------------------------------
# CONSTANTS
# ---------------------------------------------------------

const BUILDEABLE = "buildeable"
const BLOCKED = "blocked"
const ATLAS_ID = 0
const UNLOCK_TILE_POS = Vector2i(6, 0)
const NORMAL_TILE_POS = Vector2i(2, 0)

## Maximum buildeable tiles each map piece is allowed to keep.
const MAX_BUILDEABLE_PER_PIECE: int = 5

# ---------------------------------------------------------
# INTERNAL TILE KEY
# ---------------------------------------------------------

## Uniquely identifies a tile across all registered pieces.
class TileKey extends RefCounted:
	var piece: MapPiece
	var coords: Vector2i

	func _init(p_piece: MapPiece, p_coords: Vector2i) -> void:
		piece = p_piece
		coords = p_coords

# ---------------------------------------------------------
# STATE
# ---------------------------------------------------------

## All registered map pieces.
var _pieces: Array[MapPiece] = []

## Tile tracking dictionaries. Keys are String "{piece_id}:{x},{y}".
var _occupied_tiles: Dictionary = {}
var _blocked_tiles: Dictionary = {}
var _buildeable_tiles: Dictionary = {}

## Cache: maps the string key back to its TileKey for fast lookup.
var _key_to_tile: Dictionary = {}

# ---------------------------------------------------------
# LIFECYCLE
# ---------------------------------------------------------

func _ready() -> void:
	RunContext.composite_tile_map = self
	ClickEvents.tower_remove_pressed.connect(_on_tower_removed)

# ---------------------------------------------------------
# PIECE REGISTRATION
# ---------------------------------------------------------

## Registers a new map piece. Scans its tilemap for buildeable/blocked tiles.
func register_piece(piece: MapPiece) -> void:
	if piece in _pieces:
		return
	_pieces.append(piece)
	# Reduce buildeable tiles BEFORE scanning so dictionaries stay aligned
	piece.limit_buildeable_tiles(MAX_BUILDEABLE_PER_PIECE)
	_scan_piece(piece)

## Removes a piece and all its tracked tiles.
func unregister_piece(piece: MapPiece) -> void:
	if piece not in _pieces:
		return
	_pieces.erase(piece)
	# Remove all keys belonging to this piece
	var prefix: String = str(piece.get_instance_id()) + ":"
	for key in _buildeable_tiles.keys():
		if (key as String).begins_with(prefix):
			_buildeable_tiles.erase(key)
			_key_to_tile.erase(key)
	for key in _blocked_tiles.keys():
		if (key as String).begins_with(prefix):
			_blocked_tiles.erase(key)
			_key_to_tile.erase(key)
	for key in _occupied_tiles.keys():
		if (key as String).begins_with(prefix):
			_occupied_tiles.erase(key)
			_key_to_tile.erase(key)

# ---------------------------------------------------------
# PUBLIC API — Same interface as LevelTileMap
# ---------------------------------------------------------

## Returns the tile coords (local to a piece's tilemap) of the tile
## under the mouse, along with which piece it belongs to.
## Returns null if the mouse is not over any piece.
func get_mouse_tile_info() -> TileKey:
	var mouse_pos: Vector2 = get_viewport().get_mouse_position()
	# Transform from viewport to canvas (handles camera)
	var canvas_transform: Transform2D = get_viewport().get_canvas_transform()
	var global_mouse: Vector2 = canvas_transform.affine_inverse() * mouse_pos

	for piece in _pieces:
		var tm: TileMapLayer = piece.tile_map
		var local_pos: Vector2 = tm.to_local(global_mouse)
		var map_coords: Vector2i = tm.local_to_map(local_pos)
		# Only consider tiles that actually exist in this tilemap
		if tm.get_cell_source_id(map_coords) != -1:
			return TileKey.new(piece, map_coords)
	return null

## Legacy-compatible: returns the tile position as Vector2i.
## NOTE: This only makes sense when combined with the piece reference
## from get_mouse_tile_info(). For the TowerPlacer, we store both.
func get_mouse_tile_pos() -> Vector2i:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return Vector2i(-9999, -9999)
	return info.coords

## Whether the mouse is over a buildeable, non-occupied, non-blocked tile.
func is_mouse_on_buildeable_tile() -> bool:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return false
	var key: String = _make_key(info.piece, info.coords)
	if not _buildeable_tiles.has(key):
		return false
	if _occupied_tiles.has(key) or _blocked_tiles.has(key):
		return false
	return true

## Whether the mouse is over a blocked tile.
func is_mouse_on_block_tile() -> bool:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return false
	var key: String = _make_key(info.piece, info.coords)
	return _blocked_tiles.has(key)

## Returns the snapped global position of the tile under the mouse
## (suitable for placing a tower visually).
func get_current_tile_pos() -> Vector2:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return get_viewport().get_mouse_position()
	var tm: TileMapLayer = info.piece.tile_map
	var center_local: Vector2 = tm.map_to_local(info.coords)
	# Fix for 96px tile center (same offset as LevelTileMap)
	center_local.y -= 16
	return tm.to_global(center_local)

## Marks the tile under the mouse as occupied (tower placed).
## Returns the string key used internally (stored on the Tower for removal).
func set_tile_occupied_at_mouse() -> String:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return ""
	var key: String = _make_key(info.piece, info.coords)
	_occupied_tiles[key] = true
	_buildeable_tiles.erase(key)
	return key

## Marks a tile as occupied by its composite key.
func set_tile_occupied(key: String) -> void:
	_occupied_tiles[key] = true
	_buildeable_tiles.erase(key)

## Frees a tile by its composite key (tower removed).
func set_tile_free(key: String) -> void:
	if _occupied_tiles.has(key):
		_occupied_tiles.erase(key)
		_buildeable_tiles[key] = true

## Unblocks a tile by its composite key.
func unblock_tile(key: String) -> void:
	if not _blocked_tiles.has(key):
		return
	_blocked_tiles.erase(key)
	# Optionally change the visual tile
	var tile: TileKey = _key_to_tile.get(key) as TileKey
	if tile != null:
		tile.piece.tile_map.set_cell(tile.coords, ATLAS_ID, UNLOCK_TILE_POS)
	_buildeable_tiles[key] = true

## Unblocks the blocked tile under the mouse. Returns true on success.
func unblock_tile_at_mouse() -> bool:
	var info: TileKey = get_mouse_tile_info()
	if info == null:
		return false
	var key: String = _make_key(info.piece, info.coords)
	if not _blocked_tiles.has(key):
		return false
	unblock_tile(key)
	return true

## Destroys a random buildeable tile (for relic effects etc.).
func destroy_random_buildeable_tile() -> void:
	var candidates: Array = _buildeable_tiles.keys().filter(func(k: String):
		return not _blocked_tiles.has(k)
	)
	if candidates.is_empty():
		return
	var key: String = candidates[randi() % candidates.size()]
	_occupied_tiles[key] = true
	_buildeable_tiles.erase(key)
	# Change visual to non-buildable
	var tile: TileKey = _key_to_tile.get(key) as TileKey
	if tile != null:
		tile.piece.tile_map.set_cell(tile.coords, ATLAS_ID, NORMAL_TILE_POS)

# ---------------------------------------------------------
# INTERNAL HELPERS
# ---------------------------------------------------------

## Scans a piece's tilemap and populates the tracking dictionaries.
func _scan_piece(piece: MapPiece) -> void:
	var tm: TileMapLayer = piece.tile_map
	var used_cells: Array[Vector2i] = tm.get_used_cells()
	for map_coords in used_cells:
		var td: TileData = tm.get_cell_tile_data(map_coords)
		if td == null:
			continue
		var key: String = _make_key(piece, map_coords)
		_key_to_tile[key] = TileKey.new(piece, map_coords)
		if td.get_custom_data(BLOCKED) == true:
			_blocked_tiles[key] = true
		elif td.get_custom_data(BUILDEABLE) == true:
			_buildeable_tiles[key] = true

## Builds a unique string key for a tile across all pieces.
func _make_key(piece: MapPiece, coords: Vector2i) -> String:
	return "%d:%d,%d" % [piece.get_instance_id(), coords.x, coords.y]

## Called when a tower is removed — frees its tile.
func _on_tower_removed(tower: Tower) -> void:
	var key: String = tower.get("composite_tile_key") as String
	if key != null and key != "":
		set_tile_free(key)
