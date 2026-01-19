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

var towers_ids: Array[String] = []

func _init() -> void:
    ClickEvents.tower_remove_pressed.connect(tower_removed)
    RunContext.progress.current_level_changed.connect(_on_level_changed)

func reset_towers() -> void:
    _update_tower_count(Tower.Type.FIRE, 0)
    _update_tower_count(Tower.Type.LIGHTNING, 0)
    _update_tower_count(Tower.Type.FROST, 0)

# called from tower_placer to inform
func tower_added(tower: Tower) -> void:
    _update_tower_count(tower.type, towers_placed[tower.type] + 1)
    tower.id = _generate_tower_id(tower)
    tower_placed.emit(tower)

func tower_removed(tower: Tower) -> void:
    var type = tower.type
    _update_tower_count(tower.type, towers_placed[tower.type] - 1)
    towers_ids.erase(tower.id)
    tower.queue_free()

func get_tower_count(tower_type: Tower.Type) -> int:
    return towers_placed[tower_type]

func _on_level_changed(_new_level: int) -> void:
    reset_towers()

func _update_tower_count(tower_type: Tower.Type, value: int) -> void:
    towers_placed[tower_type] = value
    tower_count_change.emit(tower_type, value)
	


func _generate_tower_id(tower: Tower) -> String:
    var base_id = Tower.Type.keys()[tower.type]
    var count = towers_placed[tower.type]
    var new_id = base_id + "_" + str(count)
    while new_id in towers_ids:
        count += 1
        new_id = base_id + "_" + str(count)
    towers_ids.append(new_id)
    return new_id