class_name ArticCube extends Relic

const ARTIC_CUBE_DATA = preload("uid://br0igrelenl0f")

func apply_effect() -> void:
	EnemyDebuffManager.frost_debuff.value += 10

func get_data() -> RelicData:
	return ARTIC_CUBE_DATA
