class_name Attack extends RefCounted

var damage: float
var damage_type: DamageNumbers.Type
var source: Object
var origin: Object
var tags: Dictionary

func _init(p_damage: float, p_damage_type: DamageNumbers.Type, p_source: Object, p_origin: Object = null, p_tags: Dictionary = {}):
	damage = p_damage
	damage_type = p_damage_type
	source = p_source
	origin = p_origin if p_origin != null else p_source
	tags = p_tags
