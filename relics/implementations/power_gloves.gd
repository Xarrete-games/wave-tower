class_name PowerGloves extends Relic

const SOURCE_ID = "power_gloves"

func apply_effect() -> void:
	var source = Source.new(Source.SourceType.RELIC, SOURCE_ID)
	var buff = TowerBuffFactory.create_from_id("power_gloves_buff", source)
	if buff == null:
		return
	RunContext.towers_buffs.add_buff(buff)
	
func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(SOURCE_ID)
