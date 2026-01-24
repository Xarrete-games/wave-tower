class_name TestData extends Node

@export var initial_random_relics: int = 0
@export var initial_relics_ids: Array[String] = []
@export var run_handler: RunHandler


func _ready() -> void:
	_handle_initial_relics()

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