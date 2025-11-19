class_name TowerStats extends RefCounted

var damage: float
var attack_range: float
var attack_speed: float
var critic_chance: float
var critic_damage: float

func _init(tower: Tower) -> void:
	damage = tower.damage
	attack_range = tower.attack_range
	attack_speed = tower.attack_speed
	critic_chance = tower.critic_chance
	critic_damage = tower.critic_damage
