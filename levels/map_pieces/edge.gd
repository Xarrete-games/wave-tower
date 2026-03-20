class_name Edge extends Resource

enum Dir { NE, SE, SW, NW }
enum DirPos { TOP, MIDDLE, BOTTOM }

@export var dir: Dir = Dir.NE
@export var pos: DirPos = DirPos.MIDDLE


func _init(p_dir: Dir = Dir.NE, p_pos: DirPos = DirPos.MIDDLE) -> void:
	dir = p_dir
	pos = p_pos


## Returns a new Edge with opposite direction but same position
func get_opposite() -> Edge:
	return Edge.new(get_opposite_dir(dir), pos)


## Checks if this edge matches another (same dir and pos)
func matches(other: Edge) -> bool:
	return dir == other.dir and pos == other.pos


## Checks if this edge can connect with another (opposite dir, same pos)
func can_connect_with(other: Edge) -> bool:
	return dir == get_opposite_dir(other.dir) and pos == other.pos


## Returns the opposite direction
static func get_opposite_dir(d: Dir) -> Dir:
	match d:
		Dir.NE:
			return Dir.SW
		Dir.SE:
			return Dir.NW
		Dir.SW:
			return Dir.NE
		Dir.NW:
			return Dir.SE
		_:
			push_error("Invalid direction: %s" % [d])
			return Dir.NE


## Creates an Edge from direction only (defaults to MIDDLE position)
static func from_dir(d: Dir) -> Edge:
	return Edge.new(d, DirPos.MIDDLE)


func _to_string() -> String:
	var dir_str = ["NE", "SE", "SW", "NW"][dir]
	var pos_str = ["TOP", "MIDDLE", "BOTTOM"][pos]
	return "%s_%s" % [dir_str, pos_str]