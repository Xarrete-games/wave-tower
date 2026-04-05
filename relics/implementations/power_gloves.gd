class_name PowerGloves extends Relic

const SOURCE_ID = "power_gloves"

func apply_effect() -> void:
	var stats_modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.DAMAGE, TowerStatsModifier.Mode.FLAT, 3)
	var buff := TowerBuffStatsModifier.new(Source.new(Source.SourceType.RELIC, SOURCE_ID), stats_modifier)
	RunContext.towers_buffs.add_buff(buff)
	
func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)
