class_name PerseusFury extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "perseus_fury", ExecuteThresholdModifier.new(5))
	RunContext.towers_upgrades.add_buff(Tower.Type.RED, tower_buff)
