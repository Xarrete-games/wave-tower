class_name BlueRelic extends Relic

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.attack_range_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()
