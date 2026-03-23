class_name AttackContext
extends RefCounted

var target: Enemy
var base_damage: float
var attack: Attack
var is_critical: bool = false
var mult: float = 1.0
var tags: Dictionary = {}

func _init(p_target: Enemy, p_attack: Attack, p_is_critical: bool) -> void:
	target = p_target
	base_damage = p_attack.damage
	attack = p_attack
	is_critical = p_is_critical