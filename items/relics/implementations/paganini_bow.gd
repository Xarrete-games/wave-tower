class_name PaganiniBow extends Relic

const SOURCE_ID = "paganini_bow"

func apply_effect() -> void:
	var red_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, SOURCE_ID, DamageMultModifier.new(0.1))
	RunContext.towers_upgrades.add_buff(red_buff)

func remove_effect() -> void:
	RunContext.towers_upgrades.remove_buff(SOURCE_ID)