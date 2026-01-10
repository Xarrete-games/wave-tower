
class_name TowersBuffs extends RefCounted

signal tower_buffs_change(new_stats: TowerStatsAccumulator)
signal targeting_modes_change(new_modes: Array[Tower.TargetingMode])
signal attack_modifiers_added(new_modifier: AttackModifier)
signal attack_modifiers_removed(source_id: String)

var attack_modifiers: Array[AttackModifier] = []

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

func add_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode not in targeting_modes:
		targeting_modes.append(mode)
		targeting_modes_change.emit(targeting_modes)

func remove_targeting_mode(mode: Tower.TargetingMode) -> void:
	if mode in targeting_modes:
		targeting_modes.erase(mode)
		targeting_modes_change.emit(targeting_modes)

func add_buff(new_buff: TowerBuff) -> void:
	if new_buff.duration > 0:
		RunContext.buff_scheduler.schedule(new_buff)

	towers_buffs.append(new_buff)
	var acc = TowerStatsAccumulator.new()
	
	for buff in towers_buffs:
		buff.modifier.contribute(acc)
	towers_stats_accumulator = acc

func remove_buff(source_id: String) -> void:
	towers_buffs = towers_buffs.filter(func(buff: TowerBuff) -> bool:
		return buff.source_id != source_id
	)
	var acc = TowerStatsAccumulator.new()
	
	for buff in towers_buffs:
		buff.modifier.contribute(acc)
	towers_stats_accumulator = acc

func get_modifiers() -> Array[AttackModifier]:
	return attack_modifiers.duplicate()

func add_attack_modifier(modifier: AttackModifier) -> void:
	attack_modifiers.append(modifier)
	attack_modifiers_added.emit(modifier)

func remove_attack_modifier(source_id: String) -> void:
	attack_modifiers = attack_modifiers.filter(func(mod: AttackModifier) -> bool:
		return mod.source_id != source_id
	)
	attack_modifiers_removed.emit(source_id)

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