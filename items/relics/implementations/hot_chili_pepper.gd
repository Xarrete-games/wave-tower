class_name HotChiliPepper extends Relic

func apply_effect() -> void:
	var modifier = AllFireTowersBurnModifier.new()
	var buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "Hot Chili Pepper Burn", modifier)
	RunContext.towers_upgrades.add_buff(buff)
