class_name ValveAmplifier extends Relic

const SOURCE_ID: String = "valve_amplifier"

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(Source.new(Source.SourceType.RELIC, SOURCE_ID), AttackRangeMultModifier.new(0.1))
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)