class_name EnemyDebuffInstance extends RefCounted

var debuff: EnemyDebuff
var expire_time: float
var next_tick_time: float

func _init(p_debuff: EnemyDebuff) -> void:
	var now: float = Time.get_ticks_msec() / 1000.0
	debuff = p_debuff
	expire_time = now + p_debuff.duration
	next_tick_time = now + p_debuff.tick_interval