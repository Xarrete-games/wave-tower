class_name ArticCube extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.frost_debuff.duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.frost_debuff.duration -= 1
