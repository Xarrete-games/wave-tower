
class_name TowersUpgrades extends RefCounted

signal tower_buffs_change(tower_type: Tower.Type, new_stats: TowerBuff)

# current stats
var towers_buffs: Dictionary[Tower.Type, TowerBuff] = {
	Tower.Type.RED: RedTowerBuff.new(),
	Tower.Type.GREEN: GreenTowerBuff.new(),
	Tower.Type.BLUE: BlueTowerBuff.new(),
}

func reset_buffs() -> void:
	towers_buffs = {
		Tower.Type.RED: RedTowerBuff.new(),
		Tower.Type.GREEN: GreenTowerBuff.new(),
		Tower.Type.BLUE: BlueTowerBuff.new(),
	}
	emit_all_buffs_change()

func get_buffs(tower_type: Tower.Type) -> TowerBuff:
	return towers_buffs[tower_type]

func emit_all_buffs_change() -> void:
	for tower_type in Tower.Type.values():
		tower_buffs_change.emit(tower_type, get_buffs(tower_type))

func emit_buffs_change(tower_type: Tower.Type) -> void:
	tower_buffs_change.emit(tower_type, get_buffs(tower_type))
