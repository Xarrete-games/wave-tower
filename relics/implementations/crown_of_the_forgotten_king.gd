class_name CrownOfTheForgottenKing extends Relic

func on_wave_finished() -> void:
	RunContext.composite_tile_map.destroy_random_buildeable_tile()