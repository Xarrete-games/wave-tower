class_name DonRafaelPipe extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).duration += 1
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.BURN).duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).duration -= 1
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.BURN).duration -= 1
