@abstract
class_name EnemyDebuff extends RefCounted

signal changed
enum Type { FROST, BURN }

var type: Type
var source: Object
var value: float = 0.0:
	set(v):
		value = v
		changed.emit()
var duration: float = 0.0:
	set(v):
		duration = v
		changed.emit()
var tick_interval: float = 0.0:
	set(v):
		tick_interval = v
		changed.emit()
var extra_stacks: int = 0:
	set(v):
		extra_stacks = v
		changed.emit()

var max_stacks: int = 99


func clone(p_source: Object) -> EnemyDebuff:
	var new_enemy_debuff: EnemyDebuff
	if self is BurnDebuff:
		new_enemy_debuff = BurnDebuff.new()
	elif self is FrostDebuff:
		new_enemy_debuff = FrostDebuff.new()
	
	new_enemy_debuff.type = type
	new_enemy_debuff.value = value
	new_enemy_debuff.duration = duration
	new_enemy_debuff.tick_interval = tick_interval
	new_enemy_debuff.max_stacks = max_stacks
	new_enemy_debuff.extra_stacks = extra_stacks
	new_enemy_debuff.source = p_source
	return new_enemy_debuff

@abstract
func on_apply(enemy: Enemy)

@abstract
func on_tick(enemy: Enemy)

@abstract
func on_expire(enemy: Enemy)
