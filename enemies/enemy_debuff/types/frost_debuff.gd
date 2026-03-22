class_name FrostDebuff extends EnemyDebuff

func _init() -> void:
	type = Type.FROST
	# percentage of slowdown
	value = 5
	duration = 2
	max_stacks = 10

func clone(p_source: Source) -> EnemyDebuff:
	var cloned := FrostDebuff.new()
	_copy_base_to(cloned, p_source)
	return cloned

func on_apply(enemy: Enemy):
	enemy.speed_mult -= value / 100

func on_tick(_enemy: Enemy):
	pass
	
func on_update(_enemy: Enemy, _delta: float):
	pass

func on_expire(enemy: Enemy):
	enemy.speed_mult += value / 100
