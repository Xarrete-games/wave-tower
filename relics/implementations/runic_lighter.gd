class_name RunicLighter extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.add_modifier_from_data("runic_lighter_modifier")

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.remove_modifier_from_data("runic_lighter_modifier")
