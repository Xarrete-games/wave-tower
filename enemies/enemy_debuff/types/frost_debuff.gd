class_name FrostDebuff extends EnemyDebuff

func _init() -> void:
	type = Type.FROST
	# percentage of slowdown
	value = 0
	duration = 5
	max_stacks = 99

func on_apply(enemy: Enemy):
	enemy.speed_mult -= value / 100

func on_tick(_enemy: Enemy):
	pass
	
func on_update(_enemy: Enemy, _delta: float):
	pass

func on_expire(enemy: Enemy):
	enemy.speed_mult += value / 100
