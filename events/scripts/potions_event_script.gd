class_name PotionsEventScript extends EventScript


func get_options() -> Array[EventOptionData]:
	var options: Array[EventOptionData] = []
	var consumables: Array = DataLoader.get_all_consumables_of_type(Consumable.Type.POTION)
	
	consumables.shuffle()

	consumables = consumables.slice(0, 3)

	for consumable_data in consumables:
		var option = EventOptionData.new(consumable_data.display_name, consumable_data.create_item())
		options.append(option)
	
	return options

func handle_response(data: Variant) -> void:
	RunContext.consumables_manager.add_consumable(data as Consumable)
