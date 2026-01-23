class_name ChestEventScript extends EventScript

func get_options() -> Array[EventOptionData]:
	var option1: EventOptionData = EventOptionData.new("Open the chest", 0)
	var option2: EventOptionData = EventOptionData.new("Leave it alone", 1)
	
	return [option1, option2]

func handle_response(data: Variant) -> void:
	match data as int:
		0:
			var relic: Relic = DataLoader.get_not_used_relics(Relic.Rarity.COMMON, false).pick_random().create_item() as Relic
			RunContext.relics_manager.add_relic(relic)
		1:
			pass


