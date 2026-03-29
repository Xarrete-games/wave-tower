@abstract
class_name TowerBuffRelic extends Relic

func on_obtain() -> void:
	for tower in RunContext.towers_manager.towers:
		if _is_valid_tower(tower):
			_add_buff(tower)

func on_tower_place(tower: Tower) -> void:
	if _is_valid_tower(tower):
		_add_buff(tower)

func on_remove() -> void:
	for tower in RunContext.towers_manager.towers:
		if _is_valid_tower(tower):
			_remove_buff(tower)

@abstract
func _add_buff(tower: Tower) -> void

@abstract
func _remove_buff(tower: Tower) -> void

@abstract
func _is_valid_tower(tower: Tower) -> bool