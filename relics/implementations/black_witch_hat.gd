class_name BlackWitchHat extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.add_modifier_from_data("black_witch_hat_modifier")

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.remove_modifier_from_data("black_witch_hat_modifier")
