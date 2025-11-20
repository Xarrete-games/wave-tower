#TowerUpgrades
extends Node

signal tower_buffs_change(tower_type: Tower.TowerType, new_stats: TowerBuff)

const AMOUNT_TO_REWARD_1 = 2
const AMOUNT_TO_REWARD_2 = 4
const AMOUNT_TO_REWARD_3 = 6
const AMOUNT_TO_REWARD_4 = 8

var rewards_red: Array[Relic] = [RedRelic.new(), GreenRelic.new(), BlueRelic.new()]
var rewards_green: Array[Relic] = [RedRelic.new(), GreenRelic.new(), BlueRelic.new()]
var rewards_blue: Array[Relic] = [ArticCube.new(), ArticCube.new(), ArticCube.new()]

var rewards_ui: RewardsUI

# current stats
var towers_buffs: Dictionary[Tower.TowerType, TowerBuff] = {
	Tower.TowerType.RED: RedTowerBuff.new(),
	Tower.TowerType.GREEN: GreenTowerBuff.new(),
	Tower.TowerType.BLUE: BlueTowerBuff.new(),
}

func _ready() -> void:
	RewardsManager.tower_buffs_change.connect(_emit_all_status_change)
	TowerPlacementManager.tower_placed.connect(_on_tower_placed)

func reset_buffs() -> void:
	towers_buffs = {
		Tower.TowerType.RED: RedTowerBuff.new(),
		Tower.TowerType.GREEN: GreenTowerBuff.new(),
		Tower.TowerType.BLUE: BlueTowerBuff.new(),
	}

func get_buffs(tower_type: Tower.TowerType) -> TowerBuff:
	return towers_buffs[tower_type]

func _on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	match tower_type:
		Tower.TowerType.RED: _handle_red_updates(amount)
		Tower.TowerType.GREEN: _handle_green_updates(amount)
		Tower.TowerType.BLUE: _handle_blue_updates(amount)

func _handle_red_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_2: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_3: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_4: RewardsManager.show_rewards_ui(towers_buffs)
	
func _handle_green_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_2: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_3: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_4: RewardsManager.show_rewards_ui(towers_buffs)
	
func _handle_blue_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_2: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_3: RewardsManager.show_rewards_ui(towers_buffs)
		AMOUNT_TO_REWARD_4: RewardsManager.show_rewards_ui(towers_buffs)
		
func _emit_all_status_change(_towers_buffs: Dictionary[Tower.TowerType, TowerBuff]) -> void:
	for tower_type in Tower.TowerType.values():
		tower_buffs_change.emit(tower_type, get_buffs(tower_type))
	
