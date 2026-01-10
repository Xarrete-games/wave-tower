class_name TowerStatsHandler extends Node

signal stats_change(new_stats: TowerStats)
signal extra_stats_change(new_extra_stats: TowerExtraStats)

# base
var base_stats: TowerStats
var stats_on_level: TowerStats
var base_extra_stats: TowerExtraStats = null
# local
var local_buffs: Array[TowerBuff]
var local_stats_acc: TowerStatsAccumulator = TowerStatsAccumulator.new():
	set(new_local_stats_acc):
		local_stats_acc = new_local_stats_acc
		_update_stats()
# global
var global_stats_acc: TowerStatsAccumulator = TowerStatsAccumulator.new():
	set(new_global_stats_acc):
		global_stats_acc = new_global_stats_acc
		_update_stats()

# current stats
var stats: TowerStats = TowerStats.new()
var extra_stats: TowerExtraStats = null

var tower_type: Tower.Type
var experience_handler: ExprienceHandler

func _ready() -> void:
	RunContext.progress.current_wave_finished.connect(_on_current_wave_finished)

# initialize the stats handler with base stats, tower type and experience handler
func set_data(
	stats_configuration: TowerConfiguration,
	p_tower_type: Tower.Type, 
	p_experience_handler: ExprienceHandler) -> void:
	#base stats
	base_stats = stats_configuration.stats.duplicate()
	# stats on level
	stats_on_level = stats_configuration.stats_on_level.duplicate()
	# exprience hander
	experience_handler = p_experience_handler
	experience_handler.level_up.connect(_on_level_up)
	# tower type
	tower_type = p_tower_type
	# global buffs
	_set_global_buffs(RunContext.towers_upgrades.towers_stats_accumulator)
	RunContext.towers_upgrades.tower_buffs_change.connect(_set_global_buffs)

func add_local_buff(tower_buff: TowerBuff) -> void:
	local_buffs.append(tower_buff)
	_rebuild_local_stats_acc()

func remove_local_buff(tower_buff: TowerBuff) -> void:
	if tower_buff in local_buffs:
		local_buffs.erase(tower_buff)
		_rebuild_local_stats_acc()

func _rebuild_local_stats_acc() -> void:
	var acc = TowerStatsAccumulator.new()
	
	for buff in local_buffs:
		buff.modifier.contribute(acc)
	local_stats_acc = acc

func _on_level_up(_new_level: int) -> void:
	base_stats.add_stats(stats_on_level)
	_update_stats()

func _set_global_buffs(new_global_stats_acc: TowerStatsAccumulator) -> void:
	global_stats_acc = new_global_stats_acc


# recalculate total stats
func _update_stats() -> void:
	var total_stats_acc: TowerStatsAccumulator = global_stats_acc.merge(local_stats_acc)

	# damage
	stats.damage = (base_stats.damage + total_stats_acc.flat_damage) * (1 + total_stats_acc.damage_mult)
	# range
	stats.attack_range = (base_stats.attack_range + total_stats_acc.flat_attack_range) * (1 + total_stats_acc.attack_range_mult)
	# attack speed
	stats.attack_speed = (base_stats.attack_speed + total_stats_acc.flat_attack_speed) * (1 + total_stats_acc.attack_speed_mult)	
	# critic change
	stats.critic_chance = (base_stats.critic_chance + total_stats_acc.flat_critic_chance) * (1 + total_stats_acc.critic_chance_mult)
	# critic damage
	stats.critic_damage = (base_stats.critic_damage + total_stats_acc.flat_critic_damage) * (1 + total_stats_acc.critic_damage_mult)

	# extra stats
	var new_extra_stats: TowerExtraStats = TowerExtraStats.new()
	new_extra_stats.execute_threshold = total_stats_acc.flat_execute_threshold
	new_extra_stats.extra_hits = int(total_stats_acc.flat_extra_hits)
	new_extra_stats.double_shot_chance = total_stats_acc.flat_double_shot_chance
	new_extra_stats.all_fire_apply_burn = total_stats_acc.all_fire_apply_burn

	extra_stats = new_extra_stats

	extra_stats_change.emit(extra_stats)
	stats_change.emit(stats)

func _on_current_wave_finished() -> void:
	var buffs_to_remove: Array[TowerBuff] = []
	for buff in local_buffs:
		if buff.source_type == TowerBuff.SourceType.TEMPORAL_WAVE:
			buffs_to_remove.append(buff)
	for buff in buffs_to_remove:
			remove_local_buff(buff)