class_name SafetyHelmet extends Relic

const SAFETY_HELMET_DATA = preload("uid://cm7hdpkogsuyy")

func apply_effect() -> void:
	LiveManager.lives += 1

func get_data() -> RelicData:
	return SAFETY_HELMET_DATA
