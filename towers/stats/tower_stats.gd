class_name TowerStats extends RefCounted

var damage: float = 5
var attack_range: float = 200
var attack_speed: float = 1.0
var power_level_1: bool = false
var power_level_2: bool = false

func init_stats_base(stats_base: TowerStatsBase) -> void:
	damage = stats_base.base_damage
	attack_range = stats_base.base_attack_range
	attack_speed = stats_base.base_attack_speed
