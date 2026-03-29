class_name AttackContext
extends RefCounted

var target: Enemy
var attack: Attack

var tower: Tower

var extra_additive: float = 0.0
var extra_multiplicative: float = 0.0
var damage_cap: float = 9999
var extra_crit_chance: float = 0.0

func _init(
	p_target: Enemy, 
	p_attack: Attack,
	p_tower: Tower) -> void:
	target = p_target
	attack = p_attack
	tower = p_tower

func rebuild_attack() -> float:
	var damage = attack.damage + extra_additive
	damage *= 1 + extra_multiplicative
	return min(damage, damage_cap)

func get_crit_chance() -> float:
	var crit_chance = attack.crit_chance + extra_crit_chance
	return crit_chance