#TowerUpgrades
extends Node

signal tower_stats_change(tower_type: Tower.TowerType, stats: TowerStats)

# base stats
const RED_TOWER_BASE_STATS: TowerStatsBase = preload("uid://bndxfmba1ltf8")
const GREEN_TOWER_BASE_STATS: TowerStatsBase = preload("uid://c1d3qqqdi2oxp")
const BLUE_TOWER_BASE_STATS: TowerStatsBase = preload("uid://by0giydu700gy")
const TOWER_BASE_STATS = {
	Tower.TowerType.RED: RED_TOWER_BASE_STATS,
	Tower.TowerType.GREEN: GREEN_TOWER_BASE_STATS,
	Tower.TowerType.BLUE: BLUE_TOWER_BASE_STATS,
}

const AMOUNT_TO_REWARD_1 = 2
const AMOUNT_TO_REWARD_2 = 4
const AMOUNT_TO_REWARD_3 = 6
const AMOUNT_TO_REWARD_4 = 8

# current stats
var towers_stats = {
	Tower.TowerType.RED: TowerStats.new(),
	Tower.TowerType.GREEN: TowerStats.new(),
	Tower.TowerType.BLUE: TowerStats.new(),
}

func _ready() -> void:
	_init_stats()	

func on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	match tower_type:
		Tower.TowerType.RED: _handle_red_updates
		Tower.TowerType.GREEN: _handle_green_updates
		Tower.TowerType.BLUE: _handle_blue_updates

func get_stats(tower_type: Tower.TowerType) -> TowerStats:
	return towers_stats[tower_type]

func _init_stats() -> void:
	for tower_type in Tower.TowerType.values():
		var base_stats = TOWER_BASE_STATS[tower_type]
		get_stats(tower_type).init_stats_base(base_stats)
		
func _handle_red_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: pass
		AMOUNT_TO_REWARD_2: pass
		AMOUNT_TO_REWARD_3: pass
		AMOUNT_TO_REWARD_4: pass
	
func _handle_green_updates(amount: int) -> void:
	pass
	
func _handle_blue_updates(amount: int) -> void:
	pass
