class_name IgnitionVoltage extends Relic

const IGNITION_VOLTAGE = preload("uid://dqejgch5dthnb")

func apply_effect() -> void:
	var red_tower_buffs = TowerUpgrades.get_buffs(Tower.TowerType.RED) as RedTowerBuff
	red_tower_buffs.burn_damage += 1
	TowerUpgrades.emit_buffs_change(Tower.TowerType.RED)

func get_data() -> RelicData:
	return IGNITION_VOLTAGE
