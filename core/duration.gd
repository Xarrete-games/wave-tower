class_name Duration extends RefCounted

# duration in seconds, 0 for permanent
var seconds_duration: float = 0.0
# duration in waves, 0 for permanent
var waves_duration: int = 0

func _init(p_seconds_duration: float = 0, p_waves_duration: int = 0) -> void:
	seconds_duration = p_seconds_duration
	waves_duration = p_waves_duration