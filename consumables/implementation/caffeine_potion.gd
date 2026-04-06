class_name CaffeinePotion extends ConsumableUsable

func use() -> void:
	for tower in RunContext.towers_manager.towers:
		var source = get_source()
		var debuff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, -20)
		var buff = TowerBuffFactory.create_from_id("attack_speed_mult_buff", source, 20)
		if debuff == null or buff == null:
			continue
		var duration = Duration.new(5, 0)
		debuff.duration = duration
		buff.duration = duration
		buff.residual_buff = debuff
		tower.add_buff(buff)
