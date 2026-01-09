class_name BlackWitchHat extends Relic

func apply_effect() -> void:
	var modifier = DebuffDamageTakenModifier.new(5)
	RunContext.enemy_debuff.add_modifier(modifier)