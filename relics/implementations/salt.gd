class_name Salt extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.add_modifier_from_data("salt_damage_modifier")

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.remove_modifier_from_data("salt_damage_modifier")
