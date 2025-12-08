class_name FlowerPot extends Relic

func _init():
	super(preload("uid://crf1ar2lgywhk"))

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.GREEN) as GreenTowerBuff
	green_tower_buffs.poison_damage += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.GREEN)


