class_name Attack extends RefCounted

var damage: float
var is_critic: bool
var debuffs: Array[EnemyDebuff] = []

func _init(
	new_damage: float, 
	new_is_critic: bool,
	new_debuffs: Array[EnemyDebuff]) -> void:
	damage = new_damage
	is_critic = new_is_critic
	debuffs = new_debuffs
