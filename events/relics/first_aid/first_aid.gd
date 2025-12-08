class_name FirstAid extends Relic

func _init():
	super(preload("uid://dmu2tjsh03m2f"))

func apply_effect() -> void:
	LiveManager.lives += 10

