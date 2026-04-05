class_name LongShot extends ConsumableTargeteable

func action(target: Variant) -> void:
	var tower: Tower = target as Tower
	var source = get_source()
	var tower_buff = TowerBuffFactory.create_from_id("long_shot_buff", source)
	if tower_buff == null:
		return
	tower.add_local_buff(tower_buff)
