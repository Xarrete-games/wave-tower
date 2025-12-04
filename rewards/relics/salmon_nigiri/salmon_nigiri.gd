class_name SalmonNigiri extends Relic

const SALMON_NIGIRI_DATA = preload("uid://kgici4clfhjq")

var discount = 10

func apply_effect() -> void:
	RewardsManager.apply_discount_to_all_relics(discount)

func get_data() -> RelicData:
	return SALMON_NIGIRI_DATA
