class_name HealthBar extends Control

@onready var texture_progress_bar: TextureProgressBar = $TextureProgressBar

	
func set_max_health(value: float) -> void:
	texture_progress_bar.max_value = value
	
func update_health(new_value: float) -> void:
	texture_progress_bar.value = new_value
