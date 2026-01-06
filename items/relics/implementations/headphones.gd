class_name HeadPhones extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "headphones", DoubleShotModifier.new(0.2))
	RunContext.towers_upgrades.add_buff(Tower.Type.GREEN, tower_buff)
