class_name EchoOfVoid extends Relic

const SOURCE_ID = "echo_of_void"

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, SOURCE_ID, NumWavesModifier.new(1))
	RunContext.towers_upgrades.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_upgrades.remove_buff(SOURCE_ID)