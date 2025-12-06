class_name ArticCube extends Relic

const ARTIC_CUBE_DATA = preload("uid://br0igrelenl0f")

func apply_effect() -> void:
	var blue_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.BLUE) as BlueTowerBuff
	blue_tower_buffs.freezing_duration += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.BLUE)

func get_data() -> RelicData:
	return ARTIC_CUBE_DATA
