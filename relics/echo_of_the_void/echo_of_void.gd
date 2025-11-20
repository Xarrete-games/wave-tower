class_name EchoOfVoid extends Relic

const ECHO_OF_THE_VOID_DATA = preload("uid://bqxxbovbmgaho")

func apply_effect() -> void:
	var green_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.GREEN) as GreenTowerBuff
	green_tower_buffs.extra_waves += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.GREEN)

func get_data() -> RelicData:
	return ECHO_OF_THE_VOID_DATA
