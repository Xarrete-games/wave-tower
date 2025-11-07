class_name RangePreview extends Node2D

@export var radius: float = 100.0
@export var iso_scale_y: float = 0.5
@export var color_fill: Color = Color(0, 0.7, 1, 0.8)
@export var color_border: Color = Color(0, 0.7, 1, 0.8)
@export var line_width: float = 2.0
@export var segments: int = 64

func _draw():
	var points: PackedVector2Array = []
	for i in range(segments):
		var angle = TAU * i / segments
		points.append(Vector2(
			cos(angle) * radius,
			sin(angle) * radius * iso_scale_y
		))

	# filling the circle
	draw_colored_polygon(points, color_fill)

	# outer edge
	for i in range(segments):
		var a = points[i]
		var b = points[(i + 1) % segments]
		draw_line(a, b, color_border, line_width)
