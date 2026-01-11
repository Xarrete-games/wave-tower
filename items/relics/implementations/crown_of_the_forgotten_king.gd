class_name CrownOfTheForgottenKing extends Relic

func apply_effect() -> void:
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)
	
func remove_effect() -> void:
	RunContext.progress.current_wave_finished.disconnect(_on_wave_finished)

func _on_wave_finished() -> void:
	RunContext.level_tile_map.destroy_random_buildeable_tile()