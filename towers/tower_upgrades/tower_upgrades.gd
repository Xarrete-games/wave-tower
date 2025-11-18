#TowerUpgrades
extends Node

signal tower_buffs_change(tower_type: Tower.TowerType, new_stats: TowerBuff)

const REWARDS_UI = preload("uid://bcxsfb0ox3gmq")

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

func reset_buffs() -> void:
	towers_buffs = {
		Tower.TowerType.RED: RedTowerBuff.new(),
		Tower.TowerType.GREEN: GreenTowerBuff.new(),
		Tower.TowerType.BLUE: BlueTowerBuff.new(),
	}

func on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	match tower_type:
		Tower.TowerType.RED: _handle_red_updates(amount)
		Tower.TowerType.GREEN: _handle_green_updates(amount)
		Tower.TowerType.BLUE: _handle_blue_updates(amount)

func get_buffs(tower_type: Tower.TowerType) -> TowerBuff:
	return towers_buffs[tower_type]


func _handle_red_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: _shdow_rewards_ui(rewards_red)
		AMOUNT_TO_REWARD_2: _shdow_rewards_ui(rewards_red)
		AMOUNT_TO_REWARD_3: _shdow_rewards_ui(rewards_red)
		AMOUNT_TO_REWARD_4: _shdow_rewards_ui(rewards_red)
	
func _handle_green_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: _shdow_rewards_ui(rewards_green)
		AMOUNT_TO_REWARD_2: _shdow_rewards_ui(rewards_green)
		AMOUNT_TO_REWARD_3: _shdow_rewards_ui(rewards_green)
		AMOUNT_TO_REWARD_4: _shdow_rewards_ui(rewards_green)
	
func _handle_blue_updates(amount: int) -> void:
	match amount:
		AMOUNT_TO_REWARD_1: _shdow_rewards_ui(rewards_blue)
		AMOUNT_TO_REWARD_2: _shdow_rewards_ui(rewards_blue)
		AMOUNT_TO_REWARD_3: _shdow_rewards_ui(rewards_blue)
		AMOUNT_TO_REWARD_4:_shdow_rewards_ui(rewards_blue)
	
func _shdow_rewards_ui(rewards: Array[Relic] ) -> void:
	rewards_ui = REWARDS_UI.instantiate()
	var canvas = get_tree().root.get_node("Game").get_node("RewardsLayer")
	canvas.add_child(rewards_ui)
	rewards_ui.set_relics(rewards)
	rewards_ui.reliq_selected.connect(_on_relidc_selected)
	#RelicsManager.add_red_relic(towers_stats)
	#_emit_all_status_change()

func _on_relidc_selected(relic: Relic) -> void:
	if rewards_ui:
		rewards_ui.queue_free()
		rewards_ui = null
	RelicsManager.add_relic(towers_buffs, relic)
	_emit_all_status_change()

func _emit_all_status_change() -> void:
	for tower_type in Tower.TowerType.values():
		tower_buffs_change.emit(tower_type, get_buffs(tower_type))
	
