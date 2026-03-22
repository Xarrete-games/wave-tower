class_name VicMicrophone extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, data.id), AttackRangeMultFrostModifier.new(0.2))
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)