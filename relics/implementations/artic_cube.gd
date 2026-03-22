class_name ArticCube extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).duration -= 1
