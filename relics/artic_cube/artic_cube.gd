class_name ArticCube extends Relic

const ARTIC_CUBE_DATA = preload("uid://br0igrelenl0f")

func apply_effect(tower_buff: TowerBuff) -> void:
	var blue_tower_buffs = tower_buff as BlueTowerBuff
	blue_tower_buffs.freezing_power += 0.5

func get_data() -> RelicData:
	return ARTIC_CUBE_DATA
