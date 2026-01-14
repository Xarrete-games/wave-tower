class_name SanctuaryEventScript extends EventScript

func get_options() -> Array[EventOptionData]:
	var option1: EventOptionData = EventOptionData.new("Take offering (+50 gold)", 0)
	var option2: EventOptionData = EventOptionData.new("Pray (+10 health and 10 maximum health)", 1)
	
	return [option1, option2]

func handle_response(data: Variant) -> void:
	match data as int:
		0:
			# add gold
			RunContext.economy.add_gold(50)
		1:
			# health
			RunContext.status.add_max_health(10)
