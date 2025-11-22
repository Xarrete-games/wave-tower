class_name SalmonNigiri extends Relic

const SALMON_NIGIRI_DATA = preload("uid://kgici4clfhjq")

func apply_effect() -> void:
	RewardsManager.apply_discount_to_all_relics(5)

func get_data() -> RelicData:
	return SALMON_NIGIRI_DATA
