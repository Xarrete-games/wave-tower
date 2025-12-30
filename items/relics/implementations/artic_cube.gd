class_name ArticCube extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff.frost_debuff.duration += 1
