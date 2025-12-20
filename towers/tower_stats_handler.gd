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
		total_stats_acc =  global_stats_acc.merge(local_stats_acc)
		
		_update_stats()
# global
var global_stats_acc: TowerStatsAccumulator = TowerStatsAccumulator.new():
	set(new_global_stats_acc):
		global_stats_acc = new_global_stats_acc
		total_stats_acc =  global_stats_acc.merge(local_stats_acc)
		
		_update_stats()

# total
var total_stats_acc: TowerStatsAccumulator = TowerStatsAccumulator.new()

# current stats
var stats: TowerStats = TowerStats.new()
var extra_stats: TowerExtraStats = null

var tower_type: Tower.Type
var experience_handler: ExprienceHandler


# initialize the stats handler with base stats, tower type and experience handler
func set_data(
	stats_configuration: TowerStatsConfiguration,
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
	_set_global_buffs(tower_type, RunContext.towers_upgrades.get_stats_accumulator(tower_type))
	RunContext.towers_upgrades.tower_buffs_change.connect(_set_global_buffs)


func _on_level_up(_new_level: int) -> void:
	base_stats.add_stats(stats_on_level)
	_update_stats()

func _set_global_buffs(p_tower_type: Tower.Type,new_global_stats_acc: TowerStatsAccumulator) -> void:
	if p_tower_type != self.tower_type:
		return
	global_stats_acc = new_global_stats_acc
	_update_stats()


# recalculate total stats
func _update_stats() -> void:
	# damage
	stats.damage = (base_stats.damage + total_stats_acc.flat_damage) * (1 + total_stats_acc.damage_mult)
	# range
	stats.attack_range = (base_stats.attack_range + total_stats_acc.flat_attack_range) * (1 + total_stats_acc.attack_range_mult)
	# attck speed
	stats.attack_speed = (base_stats.attack_speed + total_stats_acc.flat_attack_speed) * (1 + total_stats_acc.attack_speed_mult)	
	# critic change
	stats.critic_chance = (base_stats.critic_chance + total_stats_acc.flat_critic_chance) * (1 + total_stats_acc.critic_chance_mult)
	# critic damage
	stats.critic_damage = (base_stats.critic_damage + total_stats_acc.flat_critic_damage) * (1 + total_stats_acc.critic_damage_mult)

	# extra stats
	var new_extra_stats: TowerExtraStats = TowerExtraStats.new()
	new_extra_stats.execute_threshold = total_stats_acc.flat_execute_threshold
	new_extra_stats.extra_waves = int(total_stats_acc.flat_extra_waves)
	new_extra_stats.double_shot_chance = total_stats_acc.flat_double_shot_chance

	extra_stats = new_extra_stats

	extra_stats_change.emit(extra_stats)
	stats_change.emit(stats)
