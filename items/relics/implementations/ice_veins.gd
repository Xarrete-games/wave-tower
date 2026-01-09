class_name IceVeins extends Relic

func apply_effect() -> void:
    RunContext.enemy_debuff.frost_debuff.extra_stacks += 1

func remove_effect() -> void:
    RunContext.enemy_debuff.frost_debuff.extra_stacks -= 1