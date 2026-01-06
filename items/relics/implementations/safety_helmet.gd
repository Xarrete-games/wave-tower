class_name SafetyHelmet extends Relic

func apply_effect() -> void:
	RunContext.progress.current_wave_changed.connect(_on_wave_changed)

func _on_wave_changed(_wave_num: int) -> void:
	RunContext.status.armor += 1
