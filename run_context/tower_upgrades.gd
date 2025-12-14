
class_name TowersUpgrades extends RefCounted

signal tower_buffs_change(tower_type: Tower.Type, new_stats: TowerBuff)
signal targeting_modes_change(new_modes: Array[Tower.TargetingMode])

# current stats
var towers_buffs: Dictionary[Tower.Type, TowerBuff] = {
	Tower.Type.RED: RedTowerBuff.new(),
	Tower.Type.GREEN: GreenTowerBuff.new(),
	Tower.Type.BLUE: BlueTowerBuff.new(),
}

var targeting_modes: Array[Tower.TargetingMode] = [
	Tower.TargetingMode.FIRST_IN_PROGRESS
]

func add_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode not in targeting_modes:
		targeting_modes.append(mode)
		targeting_modes_change.emit(targeting_modes)

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

func targeting_mode_to_string(mode: Tower.TargetingMode) -> String:
	match mode:
		Tower.TargetingMode.FIRST_IN_PROGRESS:
			return "First In Progress"
		Tower.TargetingMode.HIGHT_HP:
			return "High Health Priority"
		Tower.TargetingMode.LOW_HP:
			return "Low Health Priority"
		_:
			return "Unknown"