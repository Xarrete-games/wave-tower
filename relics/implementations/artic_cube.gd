class_name ArticCube extends Relic

const EXTRA_DURATION: float = 1.0

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	if ctx.debuff.type == EnemyDebuff.Type.FROST:
		ctx.debuff.duration += EXTRA_DURATION
