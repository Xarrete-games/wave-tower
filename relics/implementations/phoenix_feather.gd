class_name PhoenixFeather extends Relic

func on_before_die(status: Status) -> void:
	if disabled:
		return
	status.heal(10)
	RunContext.relics_manager.disable_relic(data.id)
