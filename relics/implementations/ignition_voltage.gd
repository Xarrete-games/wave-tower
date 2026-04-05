class_name IgnitionVoltage extends Relic

func apply_effect() -> void:
	var source = Source.new(Source.SourceType.RELIC, data.id)
	var tower_buff = TowerBuffFactory.create_from_id("ignition_voltage_buff", source)
	if tower_buff == null:
		return
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
