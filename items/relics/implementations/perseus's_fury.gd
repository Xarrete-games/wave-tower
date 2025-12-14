class_name PerseusFury extends Relic

func apply_effect() -> void:
	var red_tower_buffs = RunContext.towers_upgrades.get_buffs(Tower.Type.RED) as RedTowerBuff
	red_tower_buffs.extra_execute_threshold += 5
	RunContext.towers_upgrades.emit_buffs_change(Tower.Type.RED)
