#TowerCounterManager
extends Node

signal tower_count_change(tower_type: Tower.Type, amount: int, event: TowerEvent)

enum TowerEvent { PLACEMENT, SOLD, RESET }

var towers_placed: Dictionary[Tower.Type, int] = {
	Tower.Type.RED: 0,
	Tower.Type.BLUE: 0,
	Tower.Type.GREEN: 0
}

func _ready() -> void:
	ClickEvents.tower_sold_pressed.connect(_on_tower_sold)

func reset_towers() -> void:
	_update_tower_count(Tower.Type.RED, 0,TowerEvent.RESET)
	_update_tower_count(Tower.Type.GREEN, 0,TowerEvent.RESET)
	_update_tower_count(Tower.Type.BLUE, 0,TowerEvent.RESET)

# called from tower_placer to inform
func tower_added(tower: Tower) -> void:
	_update_tower_count(tower.type, towers_placed[tower.type] + 1, TowerEvent.PLACEMENT)
	
func _update_tower_count(tower_type: Tower.Type, value: int, event: TowerEvent) -> void:
	towers_placed[tower_type] = value
	tower_count_change.emit(tower_type, value, event)
	
func _on_tower_sold(tower: Tower) -> void:
	var type = tower.type
	var last_price = Price.get_sell_price(type)
	RunContext.economy.gold += last_price
	_update_tower_count(tower.type, towers_placed[tower.type] - 1, TowerEvent.SOLD)
	tower.queue_free()