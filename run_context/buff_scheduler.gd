class_name BuffScheduler extends RefCounted

func schedule(buff: TowerBuff) -> void:
	await GameState.delay(buff.duration)

	RunContext.towers_buffs.remove_buff(buff.source_id)

	if buff.residual_buff:
		add_residual(buff.residual_buff)

func add_residual(buff: TowerBuff) -> void:
	RunContext.towers_buffs.add_buff(buff)
	if buff.duration > 0:
		schedule(buff)
