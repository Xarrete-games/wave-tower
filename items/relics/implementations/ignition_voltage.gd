class_name IgnitionVoltage extends Relic

func apply_effect() -> void:
	EnemyDebuffManager.burn_debuff.value += 1
