class_name BurnDamageByTowersModifier extends DamageTakenModifier

func modify_damage(_enemy: Enemy, acc: DamageModifierAcc) -> void:
	if acc.source is not BurnDebuff:
		return

	var fire_towers_count = RunContext.towers_manager.get_tower_count(Tower.Type.RED)
	var extra_damage_mult = value * fire_towers_count
	acc.damage_mult += extra_damage_mult
