class_name MagicRing extends Relic

func _init():
	super(preload("uid://d2h7b6pi1tvlb"))

func apply_effect() -> void:
	Price.add_free_tower()
