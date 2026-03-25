class_name DebuffHandler extends Node

const BURN_COLOR =  Color.DARK_ORANGE
const FROST_COLOR = Color.AQUA
const DEFAULT_COLOR = Color.WHITE

var debuffs: Array[EnemyDebuffInstance] = []

func add_debuff(debuff: EnemyDebuff, amount: int, enemy: Enemy):
	var ctx = DebuffContext.new(debuff, amount)
	DamageHooks.on_debuff_applied(ctx, enemy)
	
	for i in range(ctx.stacks):
		if get_stacks(debuff.type) >= debuff.max_stacks:
			break
		var instance: EnemyDebuffInstance = EnemyDebuffInstance.new(debuff)
		debuffs.append(instance)
		enemy.health_bar.set_debuffs(debuffs)
		debuff.on_apply(enemy)

func update_all(enemy: Enemy):
	var now: float = Time.get_ticks_msec() / 1000.0

	for i in range(debuffs.size() - 1, -1, -1):
		var inst: EnemyDebuffInstance = debuffs[i]
		var debuff: EnemyDebuff = inst.debuff

		# tick
		if debuff.tick_interval > 0 and now >= inst.next_tick_time:
			debuff.on_tick(enemy)
			inst.next_tick_time += debuff.tick_interval

		# expire
		if now >= inst.expire_time:
			debuff.on_expire(enemy)
			debuffs.remove_at(i)
			enemy.health_bar.set_debuffs(debuffs)

func get_stacks(debuff_type: EnemyDebuff.Type) -> int:
	var count: int = 0
	for inst in debuffs:
		if inst.debuff.type == debuff_type:
			count += 1
	return count	

func has_any_defbuff() -> bool:
	return debuffs.size() > 0

func get_active_debuffs() -> Array[EnemyDebuff]:
	var result: Array[EnemyDebuff] = []
	for inst in debuffs:
		result.append(inst.debuff)
	return result
