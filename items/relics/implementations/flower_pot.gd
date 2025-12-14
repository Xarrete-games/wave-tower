class_name FlowerPot extends Relic

func apply_effect() -> void:
	var green_tower_buffs = RunContext.towers_upgrades.get_buffs(Tower.Type.GREEN) as GreenTowerBuff
	green_tower_buffs.poison_damage += 1
	RunContext.towers_upgrades.emit_buffs_change(Tower.Type.GREEN)