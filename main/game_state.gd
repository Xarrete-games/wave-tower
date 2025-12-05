#GameState
extends Node

signal state_change(state: STATE)

enum STATE { ON_MAIN_MENU, IN_GAME }

var state: STATE:
	set(value):
		state = value
		state_change.emit(state)

func is_on_main_menu() -> bool:
	return state == STATE.ON_MAIN_MENU

func reset_run() -> void:
	RelicsManager.reset_relics()
	RewardsManager.reset_rewards()
	TowerPlacementManager.reset_towers()
	TowerUpgrades.reset_buffs()
	LiveManager.lives = 5
	Score.extra_gold_dropped = 0
	Engine.time_scale = 1.0
