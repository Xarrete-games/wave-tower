class_name TowerStatsHandler extends Node

signal stats_change(new_stats)
signal buff_applied(buff: TowerBuff)
signal buff_expired(source_id: String)

# base
var base_stats
var stats_on_level
# buffs
var buffs: Array[TowerBuffStatsModifier] = []
var stats_acc: TowerStatsAccumulator = TowerStatsAccumulator.new():
	set(new_stats_acc):
		stats_acc = new_stats_acc
		_update_stats()

# current stats
var stats = null

var buff_scheduler: BuffScheduler

func _ready() -> void:
	buff_scheduler = BuffScheduler.new(RunContext.progress)
	buff_scheduler.buff_expired.connect(_on_scheduled_buff_expired)
	buff_scheduler.buff_applied.connect(_on_scheduled_buff_applied)

# initialize the stats handler with base stats, tower type and experience handler
func set_data(
	stats_configuration,
	_p_tower_type: Tower.Type,
	) -> void:
	#base stats
	base_stats = stats_configuration.stats.duplicate()
	# stats on level
	stats_on_level = stats_configuration.stats_on_level.duplicate()
	stats = base_stats.duplicate()
	
	_update_stats()

func add_buff(tower_buff: TowerBuffStatsModifier) -> void:
	buffs.append(tower_buff)
	if tower_buff.duration != null:
		buff_scheduler.schedule(tower_buff)
	_rebuild_stats_acc()

func _on_scheduled_buff_applied(buff: TowerBuff) -> void:
	buff_applied.emit(buff)

func _on_scheduled_buff_expired(buff: TowerBuff) -> void:
	buff_expired.emit(buff.source.type_id)

func remove_buff(source_id: String) -> void:
	var initial_size = buffs.size()
	buffs = buffs.filter(func(buff: TowerBuffStatsModifier) -> bool:
		return buff.source.type_id != source_id
	)
	if buffs.size() != initial_size:
		_rebuild_stats_acc()

func _rebuild_stats_acc() -> void:
	var acc = TowerStatsAccumulator.new()
	
	for buff in buffs:
		buff.contribute(acc)
	stats_acc = acc

func level_up(_new_level: int) -> void:
	base_stats.add_stats(stats_on_level)
	_update_stats()

# recalculate total stats
func _update_stats() -> void:
	var total_stats_acc: TowerStatsAccumulator = stats_acc
	# damage
	stats.damage = (base_stats.damage + total_stats_acc.flat_damage) * (1 + total_stats_acc.damage_mult)
	# range
	var attack_range_multiplier: float = 1.0 + total_stats_acc.attack_range_mult
	stats.attack_range = (base_stats.attack_range + total_stats_acc.flat_attack_range) * attack_range_multiplier
	# attack speed
	var attack_speed_multiplier: float = 1.0 + total_stats_acc.attack_speed_mult
	stats.attack_speed = (base_stats.attack_speed + total_stats_acc.flat_attack_speed) * attack_speed_multiplier	
	# critic change
	stats.critic_chance = (base_stats.critic_chance + total_stats_acc.flat_critic_chance) * (1 + total_stats_acc.critic_chance_mult)
	# critic damage
	stats.critic_damage = (base_stats.critic_damage + total_stats_acc.flat_critic_damage) * (1 + total_stats_acc.critic_damage_mult)

	stats_change.emit(stats)
