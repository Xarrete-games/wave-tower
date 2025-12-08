class_name Boniato extends Relic

func _init():
	super(preload("uid://uxa8ct8ljcw2"))

func apply_effect() -> void:
	Score.extra_gold_dropped = Score.extra_gold_dropped + 1
	LiveManager.lives -= 5