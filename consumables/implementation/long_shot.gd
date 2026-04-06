class_name LongShot extends ConsumableTargeteable

func action(target: Variant) -> void:
	var tower: Tower = target as Tower
	var source = get_source()
	var tower_buff = TowerBuffFactory.create_from_id("attack_range_mult_buff", source, 100)
	if tower_buff == null:
		return
	tower_buff.duration = Duration.new(0, 1)
	tower.add_buff(tower_buff)
