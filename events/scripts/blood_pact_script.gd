class_name BloodPactScript extends EventScript

func get_options() -> Array[EventOptionData]:
	var option1 = EventOptionData.new("Sacrifice 15 of your health to gain a powerful relic.", true)
	var option2 = EventOptionData.new("Walk away unharmed.", false)
	return [option1, option2]

func handle_response(data: Variant) -> void:
	if data as bool:
		RunContext.status.apply_damage(15)
		
		var all_relics = DataLoader.get_not_used_relics()

		var random_index = randi() % all_relics.size()
		RunContext.relics_manager.add_relic(all_relics[random_index].create_item())
	else:
		pass
