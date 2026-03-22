@abstract
class_name EnemyDebuff extends RefCounted

signal changed
enum Type { FROST, BURN }

var type: Type
var source: Source
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

@abstract
func clone(p_source: Source) -> EnemyDebuff

func _copy_base_to(target: EnemyDebuff, p_source: Source) -> void:
	target.type = type
	target.value = value
	target.duration = duration
	target.tick_interval = tick_interval
	target.max_stacks = max_stacks
	target.extra_stacks = extra_stacks
	target.source = p_source

@abstract
func on_apply(enemy: Enemy)

@abstract
func on_tick(enemy: Enemy)

@abstract
func on_expire(enemy: Enemy)
