class_name Metronome extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "metronome", AttackSpeedMultModifier.new(0.05))
	RunContext.towers_upgrades.add_buff(Tower.Type.GREEN, tower_buff)