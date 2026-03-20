class_name MapPieceData extends Resource

@export var edges: Array[Edge] = []
@export var scene: PackedScene

var is_fork: bool:
	get:
		return edges.size() > 2

func _init() -> void:
	# validate duplicate edges
	var edges_checked: Array[Edge] = []
	for e in edges:
		for checked in edges_checked:
			if e.matches(checked):
				push_error("MapPieceData has duplicate edge: %s" % [e])
				break
		edges_checked.append(e)

## Checks if this piece has an edge with the given direction and position
func has_edge(edge: Edge) -> bool:
	for e in edges:
		if e.matches(edge):
			return true
	return false


## Checks if this piece has an edge that can connect with the given edge
func has_connecting_edge(edge: Edge) -> bool:
	for e in edges:
		if e.can_connect_with(edge):
			return true
	return false


## Gets the edge that can connect with the given edge, or null
func get_connecting_edge(edge: Edge) -> Edge:
	for e in edges:
		if e.can_connect_with(edge):
			return e
	return null


## Checks if this piece has any edge with the given direction
func has_edge_dir(dir: Edge.Dir) -> bool:
	for e in edges:
		if e.dir == dir:
			return true
	return false


func get_instance() -> MapPiece:
	var instance = scene.instantiate()
	if not instance is MapPiece:
		push_error("Scene %s does not contain a MapPiece as root node" % [instance.name])
		return null
	# Deep copy edges
	var new_edges: Array[Edge] = []
	for e in edges:
		new_edges.append(Edge.new(e.dir, e.pos))
	instance.edges = new_edges
	return instance

