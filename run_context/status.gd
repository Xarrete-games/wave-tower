class_name Status extends RefCounted

signal health_change(amount: int)
signal player_died()

var health: int = 5:
	set(value):
		health = value
		health_change.emit(health)
		if health <= 0:
			player_died.emit()