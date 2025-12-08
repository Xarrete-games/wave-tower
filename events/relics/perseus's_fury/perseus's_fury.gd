class_name PerseusFury extends Relic

func _init():
	super(preload("uid://d3sm1lct1grwo"))

func apply_effect() -> void:
	var red_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.RED) as RedTowerBuff
	red_tower_buffs.extra_execute_threshold += 5
	TowerUpgrades.emit_buffs_change(Tower.TowerType.RED)
