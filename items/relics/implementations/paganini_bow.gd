class_name PaganiniBow extends Relic

func apply_effect() -> void:
	var red_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "paganini_bow", DamageMultModifier.new(0.1))
	RunContext.towers_upgrades.add_buff(red_buff)
