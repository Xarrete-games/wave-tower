class_name BurnDebuff extends EnemyDebuff

var damage_source: Source:
	get:
		return Source.new(Source.SourceType.DEBUFF, "burn_debuff")

func _init(p_data: EnemyDebuffData = null) -> void:
	type = Type.BURN
	if p_data == null:
		value = 10
		duration = 5
		tick_interval = 1
		max_stacks = 99
		return

	data = p_data
	type = p_data.debuff_type
	value = p_data.value
	duration = p_data.duration
	tick_interval = p_data.tick_interval
	max_stacks = p_data.max_stacks

func clone(p_source: Source) -> EnemyDebuff:
	var cloned := BurnDebuff.new()
	_copy_base_to(cloned, p_source)
	return cloned

func on_tick(enemy: Enemy):
	var attack = Attack.new(value, DamageNumbers.Type.SKILL, damage_source, source)
	enemy.apply_damage(attack)
