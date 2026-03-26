class_name TowerHooks

static func on_tower_placed(tower: Tower) -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_tower_placed(tower)