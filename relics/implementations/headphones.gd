class_name HeadPhones extends Relic

const SOURCE_ID = "headphones"

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, SOURCE_ID), DoubleShotModifier.new(0.2))
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)