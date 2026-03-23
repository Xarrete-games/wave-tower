class_name SaltModifier extends DamageTakenModifier

func modify_damage(enemy: Enemy, acc: DamageTakenModifierAcc) -> void:
	if enemy.get_percentage_remaining_health() <= 30:
		acc.damage_mult += value