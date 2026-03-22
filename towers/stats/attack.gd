class_name Attack extends RefCounted

var damage: float
var damage_type: DamageNumbers.Type
var source: Source
var origin_source: Source
var tags: Dictionary

func _init(p_damage: float, p_damage_type: DamageNumbers.Type, p_source: Source, p_origin: Source = null, p_tags: Dictionary = {}):
	damage = p_damage
	damage_type = p_damage_type
	source = p_source
	origin_source = p_origin if p_origin != null else p_source
	tags = p_tags
