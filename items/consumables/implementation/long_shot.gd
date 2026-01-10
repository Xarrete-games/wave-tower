class_name LongShot extends ConsumableTargeteable

func action(target: Variant) -> void:
	var tower: Tower = target as Tower
	var buff_modifier : AttackRangeMultModifier = AttackRangeMultModifier.new(1)
	var tower_buff: TowerBuff = TowerBuff.new(
		TowerBuff.SourceType.TEMPORAL_WAVE,
		id,	
		buff_modifier
	)
	tower.add_local_buff(tower_buff)