class_name DamageModifierAcc extends RefCounted

var source: Object
var origin: Object
var damage_mult: float = 1.0
var flat_damage: float = 0.0

func _init(p_source: Object, p_origin: Object) -> void:
    source = p_source
    origin = p_origin