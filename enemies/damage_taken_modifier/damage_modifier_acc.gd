class_name DamageTakenModifierAcc extends RefCounted

var damage_source: Source
var damage_origin_source: Source
var damage_mult: float = 1.0
var flat_damage: float = 0.0

func _init(p_source: Source, p_origin: Source) -> void:
    damage_source = p_source
    damage_origin_source = p_origin