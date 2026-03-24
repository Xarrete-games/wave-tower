class_name IceVeins extends Relic


func on_debuff_stack_change(debuff_type: EnemyDebuff.Type, target: Enemy, stacks: int) -> int:
	if debuff_type == EnemyDebuff.Type.FROST:
		return stacks + 1  # +1 stack de frost
	return stacks