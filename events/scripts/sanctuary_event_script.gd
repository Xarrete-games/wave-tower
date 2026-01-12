class_name SanctuaryEventScript extends EventScript

func handle_response(input: int) -> void:
	match input:
		0:
			# add gold
			RunContext.economy.add_gold(100)
		1:
			# health
			RunContext.status.add_max_health(10)
		_:
			print("Invalid option chosen in Sanctuary event.")