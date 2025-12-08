class_name SalmonNigiri extends Relic

const discount = 10

func _init():
	super(preload("uid://kgici4clfhjq"))

func apply_effect() -> void:
	RewardsManager.apply_discount_to_all_relics(discount)


