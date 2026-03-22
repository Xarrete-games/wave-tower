class_name HotChiliPepper extends Relic

const SOURCE_ID = "hot_chili_pepper"

func apply_effect() -> void:
	var modifier = AllFireTowersBurnModifier.new()
	var buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, SOURCE_ID), modifier)
	RunContext.towers_buffs.add_buff(buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)