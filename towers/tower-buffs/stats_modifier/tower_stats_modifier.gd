class_name TowerStatsModifier extends RefCounted

enum Stat {
	DAMAGE,
	ATTACK_RANGE,
	ATTACK_SPEED,
	CRITIC_CHANCE,
	CRITIC_DAMAGE,
}

enum Mode {
	FLAT,
	MULT,
}

var stat: Stat
var mode: Mode
var value: float = 0.0

func _init(p_stat: Stat, p_mode: Mode, p_value: float = 0.0) -> void:
	stat = p_stat
	mode = p_mode
	value = p_value

func contribute(acc: TowerStatsAccumulator) -> void:
	match stat:
		Stat.DAMAGE:
			if mode == Mode.FLAT:
				acc.flat_damage += value
			else:
				acc.damage_mult += value
		Stat.ATTACK_RANGE:
			if mode == Mode.FLAT:
				acc.flat_attack_range += value
			else:
				acc.attack_range_mult += value
		Stat.ATTACK_SPEED:
			if mode == Mode.FLAT:
				acc.flat_attack_speed += value
			else:
				acc.attack_speed_mult += value
		Stat.CRITIC_CHANCE:
			if mode == Mode.FLAT:
				acc.flat_critic_chance += value
			else:
				acc.critic_chance_mult += value
		Stat.CRITIC_DAMAGE:
			if mode == Mode.FLAT:
				acc.flat_critic_damage += value
			else:
				acc.critic_damage_mult += value
