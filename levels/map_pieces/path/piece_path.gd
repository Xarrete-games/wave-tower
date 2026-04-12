class_name PiecePath extends Path2D

@export_enum("NE", "SE", "SW", "NW") var start_endpoint: int = 0
@export_enum("NE", "SE", "SW", "NW") var end_endpoint: int = 0


func get_start_position() -> Vector2:
    return curve.get_point_position(0)

func get_end_position() -> Vector2:
    return curve.get_point_position(curve.get_point_count() - 1)
