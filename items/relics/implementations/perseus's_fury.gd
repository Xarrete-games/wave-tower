class_name PerseusFury extends Relic

func apply_effect() -> void:
	var red_tower_buffs = TowerUpgrades.get_buffs(Tower.Type.RED) as RedTowerBuff
	red_tower_buffs.extra_execute_threshold += 5
	TowerUpgrades.emit_buffs_change(Tower.Type.RED)
