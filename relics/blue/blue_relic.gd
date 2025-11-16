class_name BlueRelic extends Relic

const MULT = 0.1
const BLUE_RELIC_DATA = preload("uid://davvdqevvhiyk")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.attack_range *= (1.0 + MULT)

func get_data() -> RelicData:
	return BLUE_RELIC_DATA
