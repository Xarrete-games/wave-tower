class_name IgnitionVoltage extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.burn_debuff.duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.burn_debuff.duration -= 1