class_name TinfoilHat extends Relic

func on_relic_added(relic: Relic) -> void:
	if disabled or not relic.data.is_cursed:
		return

	RunContext.relics_manager.remove_relic(relic.data.id)
	disabled = true