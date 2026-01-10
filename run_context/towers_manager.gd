class_name TowersManager extends RefCounted

signal tower_count_change(tower_type: Tower.Type, amount: int)
signal tower_placed(tower: Tower)
signal tower_hovered(tower: Tower)
signal tower_unhovered(tower: Tower)

var towers_placed: Dictionary[Tower.Type, int] = {
	Tower.Type.FIRE: 0,
	Tower.Type.FROST: 0,
	Tower.Type.LIGHTNING: 0
}

func _init() -> void:
    ClickEvents.tower_remove_pressed.connect(_on_tower_removed)
    RunContext.progress.current_level_changed.connect(_on_level_changed)

func reset_towers() -> void:
    _update_tower_count(Tower.Type.FIRE, 0)
    _update_tower_count(Tower.Type.LIGHTNING, 0)
    _update_tower_count(Tower.Type.FROST, 0)

# called from tower_placer to inform
func tower_added(tower: Tower) -> void:
    _update_tower_count(tower.type, towers_placed[tower.type] + 1)
    tower_placed.emit(tower)

func get_tower_count(tower_type: Tower.Type) -> int:
    return towers_placed[tower_type]

func _on_level_changed(_new_level: int) -> void:
    reset_towers()

func _update_tower_count(tower_type: Tower.Type, value: int) -> void:
    towers_placed[tower_type] = value
    tower_count_change.emit(tower_type, value)
	
func _on_tower_removed(tower: Tower) -> void:
    var type = tower.type
    _update_tower_count(tower.type, towers_placed[tower.type] - 1)
    tower.queue_free()