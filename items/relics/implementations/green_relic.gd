class_name GreenRelic extends Relic

func apply_effect() -> void:
	for tower_buff in RunContext.towers_upgrades.towers_buffs.values():
		tower_buff.attack_speed_mult -= 0.05
	RunContext.towers_upgrades.emit_all_buffs_change()
