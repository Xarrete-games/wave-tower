class_name DebuffDamageTakenModifier extends DamageTakenModifier

func modify_damage(enemy: Enemy, acc: DamageTakenModifierAcc) -> void:
	if enemy.has_any_debuff():
		acc.flat_damage += value