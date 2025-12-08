class_name BlueRelic extends Relic

func _init():
	super(preload("uid://davvdqevvhiyk"))

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.attack_range_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()
