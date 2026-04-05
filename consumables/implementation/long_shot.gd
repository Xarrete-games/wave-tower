class_name LongShot extends ConsumableTargeteable

func action(target: Variant) -> void:
	var tower: Tower = target as Tower
	var duration = Duration.new(0, 1)
	var buff_modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_RANGE, TowerStatsModifier.Mode.MULT, 1)
	var tower_buff: TowerBuff = TowerBuffStatsModifier.new(
		Source.new(Source.SourceType.CONSUMABLE, data.id, self),
		buff_modifier,
		duration,
	)
	tower.add_local_buff(tower_buff)
