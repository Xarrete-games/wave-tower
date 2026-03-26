class_name PerseusFury extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, data.id), ExecuteThresholdModifier.new(5))
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)