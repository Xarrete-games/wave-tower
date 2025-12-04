class_name MagicRing extends Relic

const MAGIC_RING_DATA = preload("uid://d2h7b6pi1tvlb")

func apply_effect() -> void:
	Price.add_free_tower()

func get_data() -> RelicData:
	return MAGIC_RING_DATA
