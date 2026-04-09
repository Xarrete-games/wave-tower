class_name FountainsOfWishesScript extends EventScript


func get_options() -> Array[EventOptionData]:
	var gold = RunContext.economy.gold

	var epic_available = DataLoader.get_not_used_relics(BaseData.Rarity.EPIC, false).size() > 0
	var rare_available = DataLoader.get_not_used_relics(BaseData.Rarity.RARE, false).size() > 0
	var common_available = DataLoader.get_not_used_relics(BaseData.Rarity.COMMON, false).size() > 0

	var options1 = EventOptionData.new("Offer 50 coins (Receive a Common Relic)", BaseData.Rarity.COMMON, gold < 50 or not common_available)
	var options2 = EventOptionData.new("Offer 80 coins (Receive a Rare Relic)", BaseData.Rarity.RARE, gold < 80 or  not rare_available)
	var options3 = EventOptionData.new("Offer 120 coins (Receive a Epic Relic)", BaseData.Rarity.EPIC, gold < 120 or not epic_available)

	return [options1, options2, options3]

func handle_response(data: Variant) -> void:
	var rarity: int = data as int
	var relic = DataLoader.get_not_used_relics(rarity, false).pick_random().create_item()
	RunContext.relics_manager.add_relic(relic)
	var gold_cost: int = 0
	match rarity:
		BaseData.Rarity.COMMON:
			gold_cost = 50
		BaseData.Rarity.RARE:
			gold_cost = 80
		BaseData.Rarity.EPIC:
			gold_cost = 120
	RunContext.economy.spend_gold(gold_cost)
