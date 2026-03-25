class_name HotChiliPepper extends Relic

func apply_effect() -> void:
	var modifier = AllFireTowersBurnModifier.new()
	var buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, data.id), modifier)
	RunContext.towers_buffs.add_buff(buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)