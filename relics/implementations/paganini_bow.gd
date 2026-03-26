class_name PaganiniBow extends Relic

func apply_effect() -> void:
	var red_buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, data.id), DamageMultModifier.new(0.1))
	RunContext.towers_buffs.add_buff(red_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
