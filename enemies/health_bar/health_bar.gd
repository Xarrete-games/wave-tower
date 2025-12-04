class_name HealthBar extends Control

@onready var texture_progress_bar: TextureProgressBar = $TextureProgressBar

# Rango de Salud
const MIN_HEALTH: float = 40.0
const MAX_HEALTH: float = 5000.0

# Rango de Escala Visual
const MIN_SCALE: float = 1.0
const MAX_SCALE: float = 4.0

func set_max_health(value: float) -> void:
	var clamped_value = clamp(value, MIN_HEALTH, MAX_HEALTH)
	var new_scale = remap(clamped_value, MIN_HEALTH, MAX_HEALTH, MIN_SCALE, MAX_SCALE)
	self.scale = Vector2(new_scale, 1)
	
	texture_progress_bar.max_value = value
	
func update_health(new_value: float) -> void:
	texture_progress_bar.value = new_value
