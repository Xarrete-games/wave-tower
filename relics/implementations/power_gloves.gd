class_name PowerGloves extends Relic


func apply_effect() -> void:
	var source = get_source()
	var buff = TowerBuffFactory.create_from_id("power_gloves_buff", source)
	if buff == null:
		return
	RunContext.towers_buffs.add_buff(buff)
	
func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(get_source().type_id)
