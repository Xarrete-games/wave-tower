#GameState
extends Node

func reset_run() -> void:
	RelicsManager.reset_relics()
	RewardsManager.reset_rewards()
	TowerPlacementManager.reset_towers()
	TowerUpgrades.reset_buffs()
	LiveManager.lives = 5
