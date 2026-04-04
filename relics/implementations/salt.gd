class_name Salt extends Relic

const LOW_HEALTH_THRESHOLD: float = 30
const BONUS_MULTIPLIER: float = 0.3

func on_before_damage(ctx: DamageContext) -> void:
	if ctx.target.get_percentage_remaining_health() <= LOW_HEALTH_THRESHOLD:
		ctx.extra_multiplicative += BONUS_MULTIPLIER

