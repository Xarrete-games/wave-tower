class_name BurnDamageByTowersModifier extends DamageTakenModifier

func modify_damage(_enemy: Enemy, acc: DamageTakenModifierAcc) -> void:
	if acc.damage_source.type_id == "burn_debuff":
		return

	var fire_towers_count = RunContext.towers_manager.get_tower_count(Tower.Type.FIRE)
	var extra_damage_mult = value * fire_towers_count
	acc.damage_mult += extra_damage_mult
