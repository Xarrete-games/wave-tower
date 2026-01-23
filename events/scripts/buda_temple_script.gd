class_name BudaTempleScript extends EventScript

func get_options() -> Array[EventOptionData]:
	var option1: EventOptionData = EventOptionData.new("Enter the temple", 0)
	var option2: EventOptionData = EventOptionData.new("Leave it be", 1)
	
	return [option1, option2]

func handle_response(data: Variant) -> void:
	match data as int:
		0:
			var buda: Relic
			if randf() < 0.5:
				buda = DataLoader.get_relic_by_id("buda").create_item()
			else:
				buda = DataLoader.get_relic_by_id("cursed_buda").create_item()
			RunContext.relics_manager.add_relic(buda)
		1:
			pass