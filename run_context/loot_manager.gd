class_name LootManager extends RefCounted

var extra_gold: int = 0
var chance_drop_consumable: int = 50

func generate_loot_items() -> Array[LootItemData]:
	var loot_items: Array[LootItemData] = []
	
	var gold_item: LootItemData = LootItemData.new()
	gold_item.gold_amount = 50 + extra_gold

	loot_items.append(gold_item)
	if randi() % 100 >= chance_drop_consumable:
		return loot_items
		
	var consumable_item: LootItemData = LootItemData.new()
	var consumable_data: ConsumableData = DataLoader.get_all_consumables().pick_random()
	consumable_item.consumable = consumable_data

	loot_items.append(consumable_item)

	return loot_items
