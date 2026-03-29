class_name TowersBuffsManager extends RefCounted

signal tower_buffs_change(new_stats: TowerStatsAccumulator)
signal targeting_modes_change(new_modes: Array[Tower.TargetingMode])

# current buffs
var towers_buffs: Array[TowerBuff] = []
	
# current stats accumulators
var towers_stats_accumulator: TowerStatsAccumulator = TowerStatsAccumulator.new():
	set(value):
		towers_stats_accumulator = value
		tower_buffs_change.emit(towers_stats_accumulator)

var targeting_modes: Array[Tower.TargetingMode] = [
	Tower.TargetingMode.FIRST_IN_PROGRESS
]

var buff_scheduler: BuffScheduler

func _init(buff_scheduler_p: BuffScheduler) -> void:
	buff_scheduler = buff_scheduler_p
	buff_scheduler.buff_expired.connect(remove_buff)
	buff_scheduler.buff_applied.connect(add_buff)

func add_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode not in targeting_modes:
		targeting_modes.append(mode)
		targeting_modes_change.emit(targeting_modes)

func remove_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode in targeting_modes:
		targeting_modes.erase(mode)
		targeting_modes_change.emit(targeting_modes)

func add_buff(new_buff: TowerBuff) -> void:
	new_buff.scope = TowerBuff.Scope.GLOBAL
	if new_buff.duration != null:
		buff_scheduler.schedule(new_buff)

	towers_buffs.append(new_buff)
	var acc = TowerStatsAccumulator.new()
	
	for buff in towers_buffs:
		buff.modifier.contribute(acc)
	towers_stats_accumulator = acc

func remove_buff(source_id: String) -> void:
	towers_buffs = towers_buffs.filter(func(buff: TowerBuff) -> bool:
		return buff.source.type_id != source_id
	)
	var acc = TowerStatsAccumulator.new()
	
	for buff in towers_buffs:
		buff.modifier.contribute(acc)
	towers_stats_accumulator = acc

func reset_buffs() -> void:
	towers_buffs = []
	towers_stats_accumulator = TowerStatsAccumulator.new()
	
func get_buffs() -> Array[TowerBuff]:
	return towers_buffs

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