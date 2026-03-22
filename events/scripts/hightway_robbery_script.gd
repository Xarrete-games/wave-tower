class_name HighwayRobberyScript extends EventScript



func get_options() -> Array[EventOptionData]:
	var relics = RunContext.relics_manager.get_all_relics()
	var options: Array[EventOptionData] = []
	relics.shuffle()
	var relics_to_give = relics.slice(0, 3)

	for relic in relics_to_give:
		var option = EventOptionData.new("Give %s." % [relic.data.display_name], relic)
		options.append(option)

	return options

func handle_response(data: Variant) -> void:
	var relic = data as Relic
	RunContext.relics_manager.remove_relic(relic.data.id)
