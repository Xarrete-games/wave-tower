class_name BurnDebuff extends EnemyDebuff

func _init() -> void:
	type = Type.BURN
	value = 1
	duration = 5
	tick_interval = 1
	max_stacks = 99
	
func on_apply(_enemy: Enemy):
	pass

func on_tick(enemy: Enemy):
	var attack = Attack.new(value, DamageNumbers.Type.SKILL, self)
	enemy.apply_damage(attack)
	
func on_update(_enemy: Enemy, _delta: float):
	pass

func on_expire(_enemy: Enemy):
	pass
