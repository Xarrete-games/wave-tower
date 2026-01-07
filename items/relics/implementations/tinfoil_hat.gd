class_name TinfoilHat extends Relic


func apply_effect() -> void:
	RunContext.relics_manager.relic_added.connect(_on_relic_added)

func _on_relic_added(relic: Relic) -> void:
	if disabled or not relic.is_cursed:
		return

	RunContext.relics_manager.remove_relic(relic.id)
	RunContext.relics_manager.disable_relic(id)
