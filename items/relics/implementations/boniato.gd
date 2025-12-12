class_name Boniato extends Relic

func apply_effect() -> void:
	Score.extra_gold_dropped = Score.extra_gold_dropped + 1
	LiveManager.lives -= 5