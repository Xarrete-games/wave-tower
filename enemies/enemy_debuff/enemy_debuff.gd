@abstract
class_name EnemyDebuff extends RefCounted

signal changed
enum Type { FROST, BURN }

var type: Type
var value: float = 0.0:
	set(v):
		value = v
		changed.emit()
var duration: float = 0.0:
	set(v):
		duration = v
		changed.emit()
var tick_duration: float = 0.0:
	set(v):
		tick_duration = v
		changed.emit()
var time_to_tick: float = 0.0:
	set(v):
		time_to_tick = v
		changed.emit()

var max_stacks: int = 99

func clone() -> EnemyDebuff:
	var new_enemy_debuff: EnemyDebuff
	if self is BurnDebuff:
		new_enemy_debuff = BurnDebuff.new()
	elif self is FrostDebuff:
		new_enemy_debuff = FrostDebuff.new()
	
	new_enemy_debuff.type = type
	new_enemy_debuff.value = value
	new_enemy_debuff.duration = duration
	new_enemy_debuff.tick_duration = tick_duration
	new_enemy_debuff.time_to_tick = time_to_tick
	new_enemy_debuff.max_stacks = max_stacks
	return new_enemy_debuff

@abstract
func on_apply(enemy: Enemy)

@abstract
func on_tick(enemy: Enemy)

@abstract
func on_update(enemy: Enemy, delta: float)

@abstract
func on_expire(enemy: Enemy)
