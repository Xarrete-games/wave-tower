class_name FlowerPot extends Relic

const FLOWER_POT_DATA = preload("uid://crf1ar2lgywhk")

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.GREEN) as GreenTowerBuff
	green_tower_buffs.poison_damage += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.GREEN)

func get_data() -> RelicData:
	return FLOWER_POT_DATA
