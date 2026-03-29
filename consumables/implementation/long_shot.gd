class_name LongShot extends ConsumableTargeteable

func action(target: Variant) -> void:
	var tower: Tower = target as Tower
	var duration = Duration.new(0, 1)
	var buff_modifier : AttackRangeMultModifier = AttackRangeMultModifier.new(1)
	var tower_buff: TowerBuff = TowerBuff.new(
		Source.new(Source.SourceType.CONSUMABLE, data.id, self),
		buff_modifier,
		duration,
	)
	tower.add_local_buff(tower_buff)