class_name RedRelic extends Relic

const RED_RELIC_DATA = preload("uid://cj2bl454ksc7e")

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.damage_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()

func get_data() -> RelicData:
	return RED_RELIC_DATA
