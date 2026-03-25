class_name IceVeins extends Relic

func on_debuff_applied(ctx: DebuffContext, target: Enemy) -> void:
	if ctx.debuff.type == EnemyDebuff.Type.FROST:
		ctx.stacks += 1