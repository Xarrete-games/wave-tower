class_name LLibraryEventScript extends EventScript

func get_options() -> Array[EventOptionData]:
	var tome_relics = DataLoader.get_not_used_relics().filter(func(relic_data: RelicData):
		return relic_data.is_tome and not RunContext.relics_manager.has_relic(relic_data.id)
	)

	var options: Array[EventOptionData] = []
	for relic_data in tome_relics:
		var option: EventOptionData = EventOptionData.new("Acquire the %s" % relic_data.display_name, relic_data)
		options.append(option)

	return options

func handle_response(data: Variant) -> void:
	var relic_data: RelicData = data as RelicData
	if relic_data:
		var relic = relic_data.create_item()
		RunContext.relics_manager.add_relic(relic)
