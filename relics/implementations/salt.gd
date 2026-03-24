class_name Salt extends Relic

const LOW_HEALTH_THRESHOLD: float = 30
const BONUS_MULTIPLIER: float = 0.3

func on_damage_multiplicative(ctx: DamageContext, amount: float) -> float:
	if ctx.target.get_percentage_remaining_health() <= LOW_HEALTH_THRESHOLD:
		return amount + BONUS_MULTIPLIER
	return amount
