class_name LootScreenHandler extends Node

const BASE_GOLD: int = 50
const CHANCE_DROP_CONSUMABLE: int = 50

@export var loot_screen_scene: PackedScene

func show_loot_screen(event_layer: CanvasLayer) -> void:
	var loot_screen: LootScreen = loot_screen_scene.instantiate()
	event_layer.add_child(loot_screen)
	
	var loot_items = generate_loot_items()
	loot_screen.set_loot(loot_items)
	
	await  loot_screen.tree_exited

func generate_loot_items() -> Array[LootItemData]:
	var loot_items: Array[LootItemData] = []
	
	var gold_item: LootItemData = LootItemData.new()
	var ctx: LootContext = LootContext.new(BASE_GOLD, CHANCE_DROP_CONSUMABLE)
	Hooks.on_before_get_loot(ctx)
	gold_item.gold_amount = ctx.get_total_gold()

	loot_items.append(gold_item)
	if randi() % 100 >= ctx.chance_drop_consumable:
		return loot_items
		
	var consumable_item: LootItemData = LootItemData.new()
	var consumable_data: ConsumableData = DataLoader.get_all_consumables().pick_random()
	consumable_item.consumable = consumable_data

	loot_items.append(consumable_item)

	return loot_items
