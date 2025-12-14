class_name SafetyHelmet extends Relic

func apply_effect() -> void:
	RunContext.status.health += 1
