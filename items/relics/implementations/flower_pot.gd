class_name FlowerPot extends Relic

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.GREEN) as GreenTowerBuff
	green_tower_buffs.poison_damage += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.GREEN)


