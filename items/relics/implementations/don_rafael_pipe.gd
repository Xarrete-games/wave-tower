class_name DonRafaelPipe extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff.frost_debuff.duration += 1
	RunContext.enemy_debuff.burn_debuff.duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff.frost_debuff.duration -= 1
	RunContext.enemy_debuff.burn_debuff.duration += 1
