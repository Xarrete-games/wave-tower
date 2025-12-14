class_name RedRelic extends Relic

func apply_effect() -> void:
	for tower_buff in RunContext.towers_upgrades.towers_buffs.values():
		tower_buff.damage_mult += 0.1
	RunContext.towers_upgrades.emit_all_buffs_change()
