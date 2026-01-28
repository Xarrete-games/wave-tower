class_name TestData extends Node

@export var initial_random_relics: int = 0
@export var initial_relics_ids: Array[String] = []
@export var initial_consumables_ids: Array[String] = []
@export var run_handler: RunHandler
@export var initial_event: EventData = null
@export var open_loot_screen: bool = false


func _ready() -> void:
	await get_tree().create_timer(0.1).timeout
	_handle_initial_relics()
	
	if open_loot_screen:
		await run_handler.show_loot_screen()

	if initial_event:
		await run_handler.show_events_screen(initial_event)

func _handle_initial_relics() -> void:
	if initial_relics_ids.size() > 0:
		for relic_id in initial_relics_ids:
			var relic_data = DataLoader.get_relic_by_id(relic_id)
			if relic_data:
				var relic_instance = relic_data.create_item() as Relic
				RunContext.relics_manager.add_relic(relic_instance)
			else:
				push_error("[Game]: initial relic id %s not found" % relic_id)
	if initial_random_relics > 0:
		var items = RunContext.offers_manager.create_relic_offers(initial_random_relics)
		for item in items:
			var relic = item.create_item() as Relic
			RunContext.relics_manager.add_relic(relic)


	if initial_consumables_ids.size() > 0:
		for consumable_id in initial_consumables_ids:
			var consumable_data = DataLoader.get_consumable_by_id(consumable_id)
			if consumable_data:
				var consumable_instance = consumable_data.create_item() as Consumable
				RunContext.consumables_manager.add_consumable(consumable_instance)
			else:
				push_error("[Game]: initial consumable id %s not found" % consumable_id)
