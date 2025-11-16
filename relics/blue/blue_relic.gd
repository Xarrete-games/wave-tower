class_name BlueRelic extends Relic

const MULT = 0.1

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.attack_range *= (1.0 + MULT)

func get_description() -> String:
	var percent = int(MULT * 100)
	return "Increase attack range of all towers by " + str(percent) + "%"
