class_name FountainsOfWishesScript extends EventScript


func get_options() -> Array[EventOptionData]:
	var gold = RunContext.economy.gold

	var options1 = EventOptionData.new("Offer 50 coins (Receive a Common Relic)", Relic.Rarity.COMMON, gold < 50)
	var options2 = EventOptionData.new("Offer 80 coins (Receive a Rare Relic)", Relic.Rarity.RARE, gold < 80)
	var options3 = EventOptionData.new("Offer 120 coins (Receive a Epic Relic)", Relic.Rarity.EPIC, gold < 120)

	return [options1, options2, options3]

func handle_response(data: Variant) -> void:
	var rarity: int = data as int
	var relic: Relic = DataLoader.get_not_used_relics(rarity, false).pick_random().create_item() as Relic
	RunContext.relics_manager.add_relic(relic)
	var gold_cost: int = 0
	match rarity:
		Relic.Rarity.COMMON:
			gold_cost = 50
		Relic.Rarity.RARE:
			gold_cost = 80
		Relic.Rarity.EPIC:
			gold_cost = 120
	RunContext.economy.spend_gold(gold_cost)
