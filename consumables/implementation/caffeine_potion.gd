class_name CaffeinePotion extends ConsumableUsable



func use() -> void:
	var debuff = TowerBuffFactory.create_from_id("caffeine_bad_buff", get_source())
	var buff = TowerBuffFactory.create_from_id("caffeine_good_buff", get_source())
	if debuff == null or buff == null:
		return
	buff.residual_buff = debuff
	RunContext.towers_buffs.add_buff(buff)
