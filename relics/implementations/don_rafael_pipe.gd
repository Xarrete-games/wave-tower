class_name DonRafaelPipe extends Relic

const EXTRA_DURATION: float = 1.0

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	if ctx.debuff.type == EnemyDebuff.Type.FROST or ctx.debuff.type == EnemyDebuff.Type.BURN:
		ctx.debuff.duration += EXTRA_DURATION
