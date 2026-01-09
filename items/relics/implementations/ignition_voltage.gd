class_name IgnitionVoltage extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff.burn_debuff.duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff.burn_debuff.duration -= 1