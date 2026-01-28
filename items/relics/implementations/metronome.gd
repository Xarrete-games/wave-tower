class_name Metronome extends Relic

const SOURCE_ID = "metronome"

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, SOURCE_ID, AttackSpeedMultModifier.new(-0.05))
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)
