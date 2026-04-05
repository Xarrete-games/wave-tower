class_name PaganiniBow extends Relic

func apply_effect() -> void:
	var modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.DAMAGE, TowerStatsModifier.Mode.MULT, 0.1)
	var red_buff = TowerBuffStatsModifier.new(Source.new(Source.SourceType.RELIC, data.id), modifier)
	RunContext.towers_buffs.add_buff(red_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
