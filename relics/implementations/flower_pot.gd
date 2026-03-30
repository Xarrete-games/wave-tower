class_name FlowerPot extends Relic

const HEALTH_BONUS: int = 1

func on_wave_finished() -> void:
	RunContext.status.heal(HEALTH_BONUS)