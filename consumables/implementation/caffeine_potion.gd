class_name CaffeinePotion extends ConsumableUsable

func use() -> void:
	for tower in RunContext.towers_manager.towers:
		var debuff = TowerBuffFactory.create_from_id("caffeine_bad_buff", get_source())
		var buff = TowerBuffFactory.create_from_id("caffeine_good_buff", get_source())
		if debuff == null or buff == null:
			continue
		buff.residual_buff = debuff
		tower.add_buff(buff)
