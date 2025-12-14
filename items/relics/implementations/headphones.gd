class_name HeadPhones extends Relic

func apply_effect() -> void:
	var blue_tower_buffs = RunContext.towers_upgrades.get_buffs(Tower.Type.BLUE) as BlueTowerBuff
	blue_tower_buffs.double_hit_chance += 10
	RunContext.towers_upgrades.emit_buffs_change(Tower.Type.BLUE)
