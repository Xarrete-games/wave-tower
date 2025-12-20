class_name TowerStats extends RefCounted

var damage: float = 0.0
var attack_range: float = 0.0
var attack_speed: float = 0.0
var critic_chance: float = 0.0
var critic_damage: float = 0.0


func duplicate() -> TowerStats:
	var new_stats: TowerStats = TowerStats.new()
	new_stats.damage = damage
	new_stats.attack_range = attack_range
	new_stats.attack_speed = attack_speed
	new_stats.critic_chance = critic_chance
	new_stats.critic_damage = critic_damage
	return new_stats

func add_stats(other: TowerStats) -> void:
	damage += other.damage
	attack_range += other.attack_range
	attack_speed += other.attack_speed
	critic_chance += other.critic_chance
	critic_damage += other.critic_damage