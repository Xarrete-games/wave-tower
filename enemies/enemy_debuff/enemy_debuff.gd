@abstract
class_name EnemyDebuff extends RefCounted

enum Type { FROST, BURN }

var type: Type
var data: EnemyDebuffData
var source: Source
var value: float = 0.0
var duration: float = 0.0
var tick_interval: float = 0.0
var max_stacks: int = 99

func _init(p_data: EnemyDebuffData, p_source: Source) -> void:
	data = p_data
	source = source
	type = p_data.debuff_type
	value = p_data.value
	duration = p_data.duration
	tick_interval = p_data.tick_interval
	max_stacks = p_data.max_stacks

func on_apply(enemy: Enemy):
	pass

func on_tick(enemy: Enemy):
	pass

func on_expire(enemy: Enemy):
	pass

func on_damage_additive(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_multiplicative(ctx: DamageContext, amount: float) -> float:
	return amount

func on_damage_cap(ctx: DamageContext, current_cap: float) -> float:
	return current_cap
