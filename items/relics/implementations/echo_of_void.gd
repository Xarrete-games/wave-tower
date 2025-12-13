class_name EchoOfVoid extends Relic

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.Type.GREEN) as GreenTowerBuff
	green_tower_buffs.extra_waves += 1
	TowerUpgrades.emit_buffs_change(Tower.Type.GREEN)
