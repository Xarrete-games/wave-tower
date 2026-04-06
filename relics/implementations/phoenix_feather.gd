class_name PhoenixFeather extends Relic

func on_before_die(status: Status) -> void:
	if disabled:
		return
	status.heal(10)
	disabled = true
