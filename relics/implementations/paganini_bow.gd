class_name PaganiniBow extends Relic

func apply_effect() -> void:
	var source = get_source()
	var red_buff = TowerBuffFactory.create_from_id("paganinis_bow_buff", source)
	if red_buff == null:
		return
	RunContext.towers_buffs.add_buff(red_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
