class_name EchoOfVoid extends Relic

func _init():
	super(preload("uid://bqxxbovbmgaho"))

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.GREEN) as GreenTowerBuff
	green_tower_buffs.extra_waves += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.GREEN)
