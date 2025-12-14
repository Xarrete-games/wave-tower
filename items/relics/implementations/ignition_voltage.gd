class_name IgnitionVoltage extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff.burn_debuff.value += 1
