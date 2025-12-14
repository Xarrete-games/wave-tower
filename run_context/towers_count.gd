class_name TowersCount extends RefCounted

signal tower_count_change(tower_type: Tower.Type, amount: int)

var towers_placed: Dictionary[Tower.Type, int] = {
	Tower.Type.RED: 0,
	Tower.Type.BLUE: 0,
	Tower.Type.GREEN: 0
}

func _init() -> void:
    ClickEvents.tower_sold_pressed.connect(_on_tower_sold)
    RunContext.progress.current_level_changed.connect(_on_level_changed)

func reset_towers() -> void:
    _update_tower_count(Tower.Type.RED, 0)
    _update_tower_count(Tower.Type.GREEN, 0)
    _update_tower_count(Tower.Type.BLUE, 0)

# called from tower_placer to inform
func tower_added(tower: Tower) -> void:
    _update_tower_count(tower.type, towers_placed[tower.type] + 1)

func _on_level_changed(_new_level: int) -> void:
    reset_towers()

func _update_tower_count(tower_type: Tower.Type, value: int) -> void:
    towers_placed[tower_type] = value
    tower_count_change.emit(tower_type, value)
	
func _on_tower_sold(tower: Tower) -> void:
    var type = tower.type
    var last_price = Price.get_sell_price(type)
    RunContext.economy.gold += last_price
    _update_tower_count(tower.type, towers_placed[tower.type] - 1)
    tower.queue_free()