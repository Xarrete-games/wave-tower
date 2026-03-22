class_name BurnDebuff extends EnemyDebuff

var damage_source: Source:
	get:
		return Source.new(Source.SourceType.DEBUFF, "burn_debuff")

func _init() -> void:
	type = Type.BURN
	value = 10
	duration = 5
	tick_interval = 1
	max_stacks = 99

func clone(p_source: Source) -> EnemyDebuff:
	var cloned := BurnDebuff.new()
	_copy_base_to(cloned, p_source)
	return cloned

func on_apply(_enemy: Enemy):
	pass

func on_tick(enemy: Enemy):
	var attack = Attack.new(value, DamageNumbers.Type.SKILL, damage_source, source)
	enemy.apply_damage(attack)
	
func on_update(_enemy: Enemy, _delta: float):
	pass

func on_expire(_enemy: Enemy):
	pass
