class_name EchoOfVoid extends Relic

func apply_effect() -> void:
	var tower_buff = TowerBuff.new(TowerBuff.SourceType.RELIC, "echo_of_void", NumWavesModifier.new(1))
	RunContext.towers_upgrades.add_buff(Tower.Type.GREEN, tower_buff)