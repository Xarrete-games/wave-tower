class_name Boniato extends Relic

const BONIATO_DATA = preload("uid://uxa8ct8ljcw2")

func apply_effect() -> void:
	Score.extra_gold_dropped = Score.extra_gold_dropped + 1
	LiveManager.lives -= 2

func get_data() -> RelicData:
	return BONIATO_DATA
