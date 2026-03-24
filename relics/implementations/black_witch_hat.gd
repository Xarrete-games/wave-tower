class_name BlackWitchHat extends Relic

const BONUS_DAMAGE: float = 5.0

func on_damage_additive(ctx: DamageContext, amount: float) -> float:
	if ctx.target.has_any_debuff() and ctx.source.type == Source.SourceType.TOWER:
		return amount + BONUS_DAMAGE
	return amount
