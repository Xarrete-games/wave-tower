class_name RedRelic extends Relic

const MULT = 0.1
const RED_RELIC = preload("uid://bk5pyxry0ltfg")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.damage *= (1.0 + MULT)

func get_id() -> String:
	return "Red relic"

func get_description() -> String:
	var percent = int(MULT * 100)
	return "Increase damage of all towers by " + str(percent) + "%"
	
func get_texture() -> Texture2D:
	return RED_RELIC
