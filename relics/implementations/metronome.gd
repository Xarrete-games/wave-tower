class_name Metronome extends Relic

func apply_effect() -> void:
	var modifier := TowerStatsModifier.new(TowerStatsModifier.Stat.ATTACK_SPEED, TowerStatsModifier.Mode.MULT, -0.05)
	var tower_buff = TowerBuffStatsModifier.new(Source.new(Source.SourceType.RELIC, data.id), modifier)
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
