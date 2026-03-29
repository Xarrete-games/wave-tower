class_name TunelVision extends Relic

var towers_last_target_dict: Dictionary[Tower, Enemy] = {}
var hit_count_dict: Dictionary[Tower, int] = {}

const DAMAGE_MULTIPLIER_PER_HIT: float = 0.05

# increases damage by 5% for each consecutive hit on the same target
func on_before_attack(ctx: AttackContext) -> void:
	if !towers_last_target_dict.has(ctx.tower):
		towers_last_target_dict[ctx.tower] = null
		hit_count_dict[ctx.tower] = 0
		
	if ctx.target == towers_last_target_dict[ctx.tower]:
		hit_count_dict[ctx.tower] += 1
	else:
		towers_last_target_dict[ctx.tower] = ctx.target
		hit_count_dict[ctx.tower] = 0
	
	ctx.extra_multiplicative += DAMAGE_MULTIPLIER_PER_HIT * (hit_count_dict[ctx.tower])
