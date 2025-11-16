class_name GreenRelic extends Relic

const MULT = 0.1
const GREEN_RELIC_DATA = preload("uid://dqupr6bw5e16g")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.attack_speed *= (1.0 - MULT)

func get_data() -> RelicData:
	return GREEN_RELIC_DATA
