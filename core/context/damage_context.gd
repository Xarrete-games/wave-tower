class_name DamageContext extends RefCounted

var attack: Attack
var target: Enemy

var extra_additive: float = 0.0
var extra_multiplicative: float = 0.0
var damage_cap: float = 9999

func _init(p_attack: Attack, p_target: Enemy) -> void:
	attack = p_attack
	target = p_target

func get_total_damage() -> float:
	var damage = attack.damage
	damage += extra_additive
	damage *= 1.0 + extra_multiplicative
	return min(damage, damage_cap)