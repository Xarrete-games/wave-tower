class_name DebuffDamageTakenModifier extends DamageTakenModifier

var extra_damage: float = 0.1

func _init(p_extra_damage: float) -> void:
	extra_damage = p_extra_damage

func modify_damage(enemy: Enemy, acc: DamageModifierAcc) -> void:
	if enemy.has_any_debuff():
		acc.flat_damage += extra_damage