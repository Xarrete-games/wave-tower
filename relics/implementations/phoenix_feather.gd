class_name PhoenixFeather extends Relic

func on_before_die(status) -> void:
	if disabled:
		return
	status.heal(10)
	disabled = true
