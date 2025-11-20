class_name FirstAid extends Relic

const FIRST_AID_DATA = preload("uid://dmu2tjsh03m2f")

func apply_effect(tower_buff: TowerBuff) -> void:
	Score.lives += 10

func get_data() -> RelicData:
	return FIRST_AID_DATA
