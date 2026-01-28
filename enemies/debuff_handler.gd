class_name DebuffHandler extends Node

const BURN_COLOR =  Color.DARK_ORANGE
const FROST_COLOR = Color.AQUA
const DEFAULT_COLOR = Color.WHITE

var debuffs: Array[EnemyDebuffInstance] = []

func add_debuff(debuff: EnemyDebuff, amount: int, enemy: Enemy):
	var total_stacks = amount + debuff.extra_stacks
	for i in range(total_stacks):
		if get_stacks(debuff.type) >= debuff.max_stacks:
			break
		var instance: EnemyDebuffInstance = EnemyDebuffInstance.new(debuff)
		debuffs.append(instance)
		_on_add_debuff(instance, enemy)
		# on apply
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
			_on_remove_debuff(debuff.type, enemy)

func get_stacks(debuff_type: EnemyDebuff.Type) -> int:
	var count: int = 0
	for inst in debuffs:
		if inst.debuff.type == debuff_type:
			count += 1
	return count	

func has_any_defbuff() -> bool:
	return debuffs.size() > 0

func _on_add_debuff(instance: EnemyDebuffInstance, enemy: Enemy) -> void:
	var type = instance.debuff.type
	enemy.health_bar.set_debuffs(debuffs)
	match (type):
		EnemyDebuff.Type.FROST:
			pass
			# dont frost
			# if get_stacks(type) == instance.debuff.max_stacks:
			# 	enemy._is_freeze = true
			# else:
			# 	enemy._is_freeze = false

		EnemyDebuff.Type.BURN:
			pass

func _on_remove_debuff(type: EnemyDebuff.Type, enemy: Enemy) -> void:
	enemy.health_bar.set_debuffs(debuffs)
	match (type):
		EnemyDebuff.Type.FROST:
			enemy._is_freeze = false
		EnemyDebuff.Type.BURN:
			pass
