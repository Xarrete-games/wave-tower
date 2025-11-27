#TowerCounterManager
extends Node

signal tower_count_change(tower_type: Tower.TowerType, amount: int, event: TowerEvent)
signal tower_sold(tower: Tower)
signal tower_selected(tower: Tower)

enum TowerEvent { PLACEMENT, SOLD, RESET }

var towers_placed: Dictionary[Tower.TowerType, int] = {
	Tower.TowerType.RED: 0,
	Tower.TowerType.BLUE: 0,
	Tower.TowerType.GREEN: 0
}

var current_tower_selected: Tower
var is_placing: bool = false

func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_RIGHT:
		if event.pressed:
			clear_tower_selected()

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and not event.is_pressed():
			clear_tower_selected()
			get_viewport().set_input_as_handled()
		
func reset_towers_count() -> void:
	_update_tower_count(Tower.TowerType.RED, 0,TowerEvent.RESET)
	_update_tower_count(Tower.TowerType.GREEN, 0,TowerEvent.RESET)
	_update_tower_count(Tower.TowerType.BLUE, 0,TowerEvent.RESET)

func clear_tower_selected() -> void:
	current_tower_selected = null
	tower_selected.emit(current_tower_selected)

# called from tower_placer to inform
func tower_added(tower: Tower) -> void:
	_update_tower_count(tower.type, towers_placed[tower.type] + 1, TowerEvent.PLACEMENT)
	tower.selected.connect(_on_tower_selected)
	tower.stats_change.connect(_on_tower_stats_change)
	tower.sold.connect(_on_tower_sold)
	
func _update_tower_count(tower_type: Tower.TowerType, value: int, event: TowerEvent) -> void:
	towers_placed[tower_type] = value
	tower_count_change.emit(tower_type, value, event)
	
func _on_tower_selected(tower: Tower) -> void:
	current_tower_selected = tower
	tower_selected.emit(tower)

func _on_tower_sold(tower: Tower) -> void:
	var type = tower.type
	var last_price = Price.get_sell_price(type)
	Score.gold += last_price
	_update_tower_count(tower.type, towers_placed[tower.type] - 1, TowerEvent.SOLD)
	tower_sold.emit(tower)
	tower.queue_free()

func _on_tower_stats_change(tower: Tower) -> void:
	if tower == current_tower_selected:
		tower_selected.emit(tower)
	
