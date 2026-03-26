class_name ProgressHooks

static func on_wave_finished() -> void:
	for relic in RunContext.relics_manager.get_all_relics():
		relic.on_wave_finished()
