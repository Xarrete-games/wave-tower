class_name HotChiliPepper extends TowerBuffRelic

func _add_buff(tower: Tower) -> void:
	var fire_tower = tower as FireTower
	fire_tower.apply_burn = true

func _remove_buff(tower: Tower) -> void:
	var fire_tower = tower as FireTower
	fire_tower.apply_burn = false

func _is_valid_tower(tower: Tower) -> bool:
	return tower.data.type == Tower.Type.FIRE and tower is not WildFireTower
