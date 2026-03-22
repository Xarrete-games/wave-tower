class_name IceVeins extends Relic

func apply_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).extra_stacks += 1

func remove_effect() -> void:
	RunContext.enemy_debuff_manager.get_template(EnemyDebuff.Type.FROST).extra_stacks -= 1