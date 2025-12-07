class_name Attack extends RefCounted

var damage: float
var is_critic: bool

func _init(new_damage: float, new_is_critic: bool):
	damage = new_damage
	is_critic = new_is_critic
