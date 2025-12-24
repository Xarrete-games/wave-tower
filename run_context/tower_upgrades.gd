
class_name TowersUpgrades extends RefCounted

signal tower_buffs_change(tower_type: Tower.Type, new_stats: TowerStatsAccumulator)
signal targeting_modes_change(new_modes: Array[Tower.TargetingMode])

# current buffs
var towers_buffs: Dictionary[Tower.Type, Array] = {
	Tower.Type.RED: [],
	Tower.Type.GREEN: [],
	Tower.Type.BLUE: [],
}

# current stats accumulators
var towers_stats_accumulator: Dictionary[Tower.Type, TowerStatsAccumulator] = {
	Tower.Type.RED: TowerStatsAccumulator.new(),
	Tower.Type.GREEN: TowerStatsAccumulator.new(),
	Tower.Type.BLUE: TowerStatsAccumulator.new(),
}

var targeting_modes: Array[Tower.TargetingMode] = [
	Tower.TargetingMode.FIRST_IN_PROGRESS
]

func add_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode not in targeting_modes:
		targeting_modes.append(mode)
		targeting_modes_change.emit(targeting_modes)

func add_global_buff(buff: TowerBuff) -> void:
	for tower_type in Tower.Type.values():
		add_buff(tower_type, buff)

func add_buff(tower_type: Tower.Type, new_buff: TowerBuff) -> void:
	towers_buffs[tower_type].append(new_buff)
	var acc = TowerStatsAccumulator.new()
	
	for buff in towers_buffs[tower_type]:
		buff.modifier.contribute(acc)
	towers_stats_accumulator[tower_type] = acc
	emit_buffs_change(tower_type)

func reset_buffs() -> void:
	towers_buffs = {
		Tower.Type.RED: [],
		Tower.Type.GREEN: [],
		Tower.Type.BLUE: [],
	}
	towers_stats_accumulator = {
		Tower.Type.RED: TowerStatsAccumulator.new(),
		Tower.Type.GREEN: TowerStatsAccumulator.new(),
		Tower.Type.BLUE: TowerStatsAccumulator.new(),
	}
	
func get_buffs(tower_type: Tower.Type) -> Array[TowerBuff]:
	return towers_buffs[tower_type]

func get_stats_accumulator(tower_type: Tower.Type) -> TowerStatsAccumulator:
	return towers_stats_accumulator[tower_type]

func emit_buffs_change(tower_type: Tower.Type) -> void:
	tower_buffs_change.emit(tower_type, get_stats_accumulator(tower_type))

func targeting_mode_to_string(mode: Tower.TargetingMode) -> String:
	match mode:
		Tower.TargetingMode.FIRST_IN_PROGRESS:
			return "Progress"
		Tower.TargetingMode.HIGH_HP:
			return "High Health"
		Tower.TargetingMode.LOW_HP:
			return "Low Health"
		_:
			return "Unknown"