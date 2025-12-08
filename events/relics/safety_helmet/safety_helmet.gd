class_name SafetyHelmet extends Relic

func _init():
	super(preload("uid://cm7hdpkogsuyy"))

func apply_effect() -> void:
	LiveManager.lives += 1
