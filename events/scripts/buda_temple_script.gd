class_name BudaTempleScript extends EventScript

func handle_response(input: int) -> void:
	match input:
		0:
			var buda: Relic
			if randf() < 0.5:
				buda = DataLoader.get_relic_by_id("buda").create_item()
			else:
				buda = DataLoader.get_relic_by_id("cursed_buda").create_item()
			RunContext.relics_manager.add_relic(buda)
		1:
			print("Player chose option 1 in Buda Temple event.")
		_:
			print("Invalid option chosen in Buda Temple event.")