class_name Attack extends RefCounted

var damage: float
var is_critical: bool = false
var is_execution: bool = false
var hits: int = 1
var bounces: int = 0
var effects: Array[EnemyEffect] = []
var source: Source

var tags: Dictionary

func _init(
	p_damage: float,
	p_source: Source, 
	p_is_critical: bool = false,
	p_is_execution: bool = false, 
	p_tags: Dictionary = {}):
	damage = p_damage
	is_critical = p_is_critical
	is_execution = p_is_execution
	source = p_source
	tags = p_tags
