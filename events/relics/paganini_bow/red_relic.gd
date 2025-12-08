class_name RedRelic extends Relic

func _init():
	super(preload("uid://cj2bl454ksc7e"))

func apply_effect() -> void:
	for tower_buff in TowerUpgrades.towers_buffs.values():
		tower_buff.damage_mult += 0.1
	TowerUpgrades.emit_all_buffs_change()
