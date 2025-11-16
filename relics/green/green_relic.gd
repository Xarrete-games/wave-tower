class_name GreenRelic extends Relic

const MULT = 0.1
const GREEN_RELIC = preload("uid://cwqmn01etch8n")

func apply_effect(tower_stats: TowerStats) -> void:
	tower_stats.attack_speed *= (1.0 + MULT)

func get_id() -> String:
	return "Green Relic"

func get_description() -> String:
	var percent = int(MULT * 100)
	return "Increase attack speed of all towers by " + str(percent) + "%"

func get_texture() -> Texture2D:
	return GREEN_RELIC
