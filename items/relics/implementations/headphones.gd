class_name HeadPhones extends Relic

func apply_effect() -> void:
	var blue_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.BLUE) as BlueTowerBuff
	blue_tower_buffs.double_hit_chance += 10
	TowerUpgrades.emit_buffs_change(Tower.TowerType.BLUE)
