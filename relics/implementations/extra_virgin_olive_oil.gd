class_name ExtraVirginOliveOil extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.BURN).duration += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.BURN).duration -= 1