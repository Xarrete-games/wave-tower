extends PointLight2D

@export var velocidad: float = 6.0    # rapidez de la oscilación
@export var escala_min: float = 0.7
@export var escala_max: float = 1.0

var t := 0.0

func _process(delta: float) -> void:
	t += delta * velocidad
	var s = (sin(t) + 1.0) * 0.5   # convierte sin(-1..1) → 0..1
	texture_scale = lerp(escala_min, escala_max, s)
