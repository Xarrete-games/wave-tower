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

# current stats
var towers_buffs: Dictionary[Tower.TowerType, TowerBuff] = {
	Tower.TowerType.RED: RedTowerBuff.new(),
	Tower.TowerType.GREEN: GreenTowerBuff.new(),
	Tower.TowerType.BLUE: BlueTowerBuff.new(),
}

func _ready() -> void:
	pass

func reset_buffs() -> void:
	towers_buffs = {
		Tower.TowerType.RED: RedTowerBuff.new(),
		Tower.TowerType.GREEN: GreenTowerBuff.new(),
		Tower.TowerType.BLUE: BlueTowerBuff.new(),
	}
	emit_all_buffs_change()

func get_buffs(tower_type: Tower.TowerType) -> TowerBuff:
	return towers_buffs[tower_type]

func emit_all_buffs_change() -> void:
	for tower_type in Tower.TowerType.values():
		tower_buffs_change.emit(tower_type, get_buffs(tower_type))

func emit_buffs_change(tower_type: Tower.TowerType) -> void:
	tower_buffs_change.emit(tower_type, get_buffs(tower_type))
