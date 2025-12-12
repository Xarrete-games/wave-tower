class_name RedRelic extends Relic

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.damage_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()
