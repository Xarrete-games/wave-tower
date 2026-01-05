class_name TunelVisionModifier extends AttackModifier

const DAMAGE_MULTIPLIER_PER_HIT: float = 0.05

var last_target : Enemy = null
var hit_count : int = 0

# increases damage by 5% for each consecutive hit on the same target
func on_before_hit(ctx: AttackContext) -> void:
	if ctx.target != last_target:
		print("Resetting hit count for new target")
		last_target = ctx.target
		hit_count = 0
	
	print("Hit count for target %s: %d" % [ctx.target, hit_count])
	hit_count += 1
	var damage_increase = DAMAGE_MULTIPLIER_PER_HIT * (hit_count - 1)
	ctx.attack.damage += ctx.base_damage * damage_increase
