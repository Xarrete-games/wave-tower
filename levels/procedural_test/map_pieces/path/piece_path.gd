class_name PiecePath extends Path2D

@export var start_endpoint: MapPiece.Dir
@export var end_endpoint: MapPiece.Dir


func get_start_position() -> Vector2:
    return curve.get_point_position(0)

func get_end_position() -> Vector2:
    return curve.get_point_position(curve.get_point_count() - 1)
