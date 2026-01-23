class_name LootManager extends RefCounted

func generate_loot_items() -> Array[LootItemData]:
	var loot_items: Array[LootItemData] = []
	
	var gold_item: LootItemData = LootItemData.new()
	gold_item.gold_amount = 50

	loot_items.append(gold_item)

	var consumable_item: LootItemData = LootItemData.new()
	var consumable_data: ConsumableItemData = DataLoader.get_all_consumables().pick_random()
	consumable_item.consumable = consumable_data

	loot_items.append(consumable_item)

	return loot_items
