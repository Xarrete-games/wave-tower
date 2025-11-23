#Rewarsmanager
extends Node

signal tower_buffs_change(towers_buffs: Dictionary[Tower.TowerType, TowerBuff])

const REWARDS_UI = preload("uid://bcxsfb0ox3gmq")

const AMOUNT_TO_REWARD_1 = 2
const AMOUNT_TO_REWARD_2 = 4
const AMOUNT_TO_REWARD_3 = 6
const AMOUNT_TO_REWARD_4 = 8

var relics_list: Array[Relic] = [
	RedRelic.new(), GreenRelic.new(), BlueRelic.new(),
	ArticCube.new(), EchoOfVoid.new(), PerseusFury.new(),
	FirstAid.new(), MagicRing.new(), Boniato.new(),
	SalmonNigiri.new(), FlowerPot.new(), HeadPhones.new(),
	IgnitionVoltage.new()
]

var all_rewards: Array[Relic] = relics_list.duplicate()
var rewards_ui: RewardsUI
var towers_buffs: Dictionary[Tower.TowerType, TowerBuff]

func _ready() -> void:
	RelicsManager.relics_change.connect(_on_relics_change)

func reset_rewards() -> void:
	all_rewards = relics_list.duplicate()

func show_rewards_ui(new_towers_buffs: Dictionary[Tower.TowerType, TowerBuff]) -> void:
	towers_buffs = new_towers_buffs
	rewards_ui = REWARDS_UI.instantiate()
	var rewards = _get_rewards()
	var canvas = get_tree().root.get_node("Game").get_node("RewardsLayer")
	canvas.add_child(rewards_ui)
	rewards_ui.set_relics(rewards)
	rewards_ui.reliq_selected.connect(_on_relidc_selected)

func apply_discount_to_all_relics(discount: int) -> void:
	for relic: Relic in all_rewards:
		relic.price = round(relic.price - (relic.price * (discount / 100.0)))

func _get_rewards() -> Array[Relic]:
	randomize()
	var rewars_copy = all_rewards.duplicate()
	rewars_copy.shuffle()
	return rewars_copy.slice(0 ,3)
	
func _on_relidc_selected(relic: Relic) -> void:
	if rewards_ui:
		rewards_ui.queue_free()
		rewards_ui = null
	RelicsManager.add_relic(relic)
	tower_buffs_change.emit(towers_buffs)

func _on_relics_change(relics: Array[Relic]) -> void:
	for relic in relics:
		if relic.amount == relic.max_stack:
			_remove_reward(relic.id)

func _remove_reward(id: String) -> void:
	for i in range(all_rewards.size()):
		var current_reward = all_rewards[i]
		if current_reward.id == id:
			all_rewards.remove_at(i)
			return
