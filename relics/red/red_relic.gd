class_name RedRelic extends Relic

const MULT = 0.1
const RED_RELIC_DATA = preload("uid://cj2bl454ksc7e")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.damage *= (1.0 + MULT)

func get_data() -> RelicData:
	return RED_RELIC_DATA
