class_name BlackWitchHat extends Relic

const BONUS_DAMAGE: float = 5.0

func on_before_damage(ctx: DamageContext) -> void:
	if ctx.target.has_any_debuff() and ctx.attack.source.type == Source.SourceType.TOWER:
		ctx.extra_additive += BONUS_DAMAGE	
