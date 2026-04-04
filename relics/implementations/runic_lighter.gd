class_name RunicLighter extends Relic

const DAMAGE_PER_FIRE_TOWER: float = 0.1

func on_before_attack(ctx: AttackContext) -> void:
	if ctx.source.type == Source.SourceType.DEBUFF and ctx.source.type_id == "burn_debuff":
		var fire_towers = RunContext.towers_manager.get_tower_count(Tower.Type.FIRE)
		var extra_mult = 1.0 + (DAMAGE_PER_FIRE_TOWER * fire_towers)
		ctx.extra_multiplicative += extra_mult