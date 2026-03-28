class_name DamageContext extends RefCounted

var attack: Attack
var target: Enemy

func _init(p_attack: Attack, p_target: Enemy) -> void:
	attack = p_attack
	target = p_target