class_name Status extends RefCounted

signal health_change(amount: int)
signal armor_change(amount: int)
signal max_health_change(amount: int)
signal player_died()


var max_health: int = 20:
	set(value):
		max_health = value
		max_health_change.emit(max_health)

var health: int = max_health:
	set(value):
		health = min(value, max_health)
		health_change.emit(health)
		if health <= 0:
			player_died.emit()

var armor: int = 0:
	set(value):
		armor = value
		armor_change.emit(armor)

func _init() -> void:
	RunContext.progress.current_wave_finished.connect(_on_wave_finished)


func apply_damage(amount: int) -> void:
	if amount <= 0:
		return

	var remaining_damage: int = amount

	if armor > 0:
		var absorbed: int = min(armor, remaining_damage)
		armor -= absorbed
		remaining_damage -= absorbed
		armor_change.emit(armor)

	if remaining_damage > 0:
		health -= remaining_damage


func _on_wave_finished() -> void:
	armor = 0