class_name Salt extends Relic

func apply_effect() -> void:
	var modifier = SaltModifier.new(DamageTakenModifier.SourceType.RELIC, id, 0.15)
	RunContext.enemy_debuff_manager.add_modifier(modifier)

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.remove_modifier(id)