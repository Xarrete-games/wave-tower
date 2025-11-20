class_name GreenRelic extends Relic

const GREEN_RELIC_DATA = preload("uid://dqupr6bw5e16g")

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.attack_speed_mult -= 0.1
	TowerUpgrades.emit_all_buffs_change()

func get_data() -> RelicData:
	return GREEN_RELIC_DATA
