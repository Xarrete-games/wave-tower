class_name MapPieceData extends Resource

@export var edges: Array[MapPiece.Dir] = []
@export var scene: PackedScene

func _init() -> void:
	# validate duplicates edges
	var unique_edges = edges.duplicate()
	var edges_checked = []
	for e in unique_edges:
		if edges_checked.has(e):
			push_error("MapPieceData has duplicate edge: %s" % [e])
		else:
			edges_checked.append(e)

func get_instance() -> MapPiece:
	var instance = scene.instantiate()
	if not instance is MapPiece:
		push_error("Scene %s does not contain a MapPiece as root node" % [scene])
		return null
	instance.edges = edges.duplicate()
	return instance
