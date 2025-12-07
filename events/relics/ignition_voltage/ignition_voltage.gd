class_name IgnitionVoltage extends Relic

const IGNITION_VOLTAGE = preload("uid://dqejgch5dthnb")

func apply_effect() -> void:
	EnemyDebuffManager.burn_debuff.value += 1

func get_data() -> RelicData:
	return IGNITION_VOLTAGE
