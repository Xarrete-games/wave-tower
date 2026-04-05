class_name Metronome extends Relic

func apply_effect() -> void:
	var source = get_source()
	var tower_buff = TowerBuffFactory.create_from_id("metronome_buff", source)
	if tower_buff == null:
		return
	RunContext.towers_buffs.add_buff(tower_buff)

func remove_effect() -> void:
	RunContext.towers_buffs.remove_buff(data.id)
