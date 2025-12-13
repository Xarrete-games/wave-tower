#TowerUpgrades
extends Node

signal tower_buffs_change(tower_type: Tower.Type, new_stats: TowerBuff)

const AMOUNT_TO_REWARD_1 = 2
const AMOUNT_TO_REWARD_2 = 4
const AMOUNT_TO_REWARD_3 = 6
const AMOUNT_TO_REWARD_4 = 8

# current stats
var towers_buffs: Dictionary[Tower.Type, TowerBuff] = {
	Tower.Type.RED: RedTowerBuff.new(),
	Tower.Type.GREEN: GreenTowerBuff.new(),
	Tower.Type.BLUE: BlueTowerBuff.new(),
}

func _ready() -> void:
	pass

func reset_buffs() -> void:
	towers_buffs = {
		Tower.Type.RED: RedTowerBuff.new(),
		Tower.Type.GREEN: GreenTowerBuff.new(),
		Tower.Type.BLUE: BlueTowerBuff.new(),
	}
	emit_all_buffs_change()

func get_buffs(tower_type: Tower.Type) -> TowerBuff:
	return towers_buffs[tower_type]

func emit_all_buffs_change() -> void:
	for tower_type in Tower.Type.values():
		tower_buffs_change.emit(tower_type, get_buffs(tower_type))

func emit_buffs_change(tower_type: Tower.Type) -> void:
	tower_buffs_change.emit(tower_type, get_buffs(tower_type))
