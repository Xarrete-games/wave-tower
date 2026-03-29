class_name PerseusFury extends TowerBuffRelic

const EXTRA_EXECUTE_THRESHOLD = 5

func _add_buff(tower: Tower) -> void:
	var fire_laser_tower = tower as FireLaserTower
	fire_laser_tower.execute_threshold += EXTRA_EXECUTE_THRESHOLD

func _remove_buff(tower: Tower) -> void:
	var fire_laser_tower = tower as FireLaserTower	
	fire_laser_tower.execute_threshold -= EXTRA_EXECUTE_THRESHOLD

func _is_valid_tower(tower: Tower) -> bool:
	return tower is FireLaserTower