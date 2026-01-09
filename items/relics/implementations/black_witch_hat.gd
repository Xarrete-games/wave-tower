class_name BlackWitchHat extends Relic

const SOURCE_ID = "black_witch_hat"

func apply_effect() -> void:
	var modifier = DebuffDamageTakenModifier.new(DamageTakenModifier.SourceType.RELIC, SOURCE_ID, 5)
	RunContext.enemy_debuff.add_modifier(modifier)

func remove_effect() -> void:
	RunContext.enemy_debuff.remove_modifier(SOURCE_ID)