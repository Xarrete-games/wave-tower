class_name FrostDebuff extends EnemyDebuff

func _init(p_data: EnemyDebuffData = null) -> void:
	type = Type.FROST
	if p_data == null:
		value = 5
		duration = 2
		max_stacks = 10
		return

	data = p_data
	type = p_data.debuff_type
	value = p_data.value
	duration = p_data.duration
	tick_interval = p_data.tick_interval
	max_stacks = p_data.max_stacks

func clone(p_source: Source) -> EnemyDebuff:
	var cloned := FrostDebuff.new()
	_copy_base_to(cloned, p_source)
	return cloned

func on_apply(enemy: Enemy):
	enemy.speed_mult -= value / 100

func on_expire(enemy: Enemy):
	enemy.speed_mult += value / 100
