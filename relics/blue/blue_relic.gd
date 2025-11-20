class_name BlueRelic extends Relic

const MULT = 0.1
const BLUE_RELIC_DATA = preload("uid://davvdqevvhiyk")

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.attack_range_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()
	
func get_data() -> RelicData:
	return BLUE_RELIC_DATA
