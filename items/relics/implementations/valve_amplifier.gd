class_name ValveAmplifier extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "valve_amplifier", AttackRangeMultModifier.new(0.1))
	RunContext.towers_upgrades.add_buff(Tower.Type.BLUE, tower_buff)
