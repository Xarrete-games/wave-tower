class_name BlueRelic extends Relic

const MULT = 0.1
const BLUE_RELIC = preload("uid://cubmexn5ct6ht")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.attack_range *= (1.0 + MULT)

func get_id() -> String:
	return "Blue Relic"

func get_description() -> String:
	var percent = int(MULT * 100)
	return "Increase attack range of all towers by " + str(percent) + "%"

func get_texture() -> Texture2D:
	return BLUE_RELIC
