#TowerCounterManager
extends Node

signal tower_placed(tower_type: Tower.TowerType, amount: int)

var towers_placed: Dictionary[Tower.TowerType, int] = {
	Tower.TowerType.RED: 0,
	Tower.TowerType.BLUE: 0,
	Tower.TowerType.GREEN: 0
}

func reset_towers_count() -> void:
	_update_tower_count(Tower.TowerType.RED, 0)
	_update_tower_count(Tower.TowerType.GREEN, 0)
	_update_tower_count(Tower.TowerType.BLUE, 0)

# called from tower_placer to inform
func tower_added(tower_type: Tower.TowerType) -> void:
	_update_tower_count(tower_type, towers_placed[tower_type] + 1)

func _update_tower_count(tower_type: Tower.TowerType, value: int) -> void:
	towers_placed[tower_type] = value
	tower_placed.emit(tower_type, value)
