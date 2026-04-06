class_name Status extends RefCounted

signal health_change(amount: int)
signal armor_change(amount: int)
signal max_health_change(amount: int)
signal player_died()

var max_health: int = 20:
	set(value):
		max_health = value
		if health > max_health:
			health = max_health
		max_health_change.emit(max_health)

var health: int = max_health:
	set(value):
		health = min(value, max_health)
		health_change.emit(health)
		if health <= 0:
			Hooks.on_before_die(self)

			if health <= 0:
				player_died.emit()

var armor: int = 0:
	set(value):
		armor = value
		armor_change.emit(armor)

var progress: RunProgress
var relics_manager: RelicsManager

func _init(p_progress: RunProgress, p_relics_manager: RelicsManager) -> void:
	progress = p_progress
	relics_manager = p_relics_manager
	progress.current_wave_finished.connect(_on_wave_finished)

func heal(amount: int) -> void:
	if amount <= 0:
		return

	health += amount

func add_amor(amount: int) -> void:
	if amount <= 0:
		return

	armor += amount

func add_max_health(amount: int) -> void:
	if amount <= 0:
		return

	max_health += amount
	health += amount

func apply_damage(amount: int) -> void:
	if amount <= 0:
		return

	var remaining_damage: int = amount
	var armor_block_damage: bool = armor >= remaining_damage 

	if armor > 0:
		var absorbed: int = min(armor, remaining_damage)
		armor -= absorbed
		remaining_damage -= absorbed
		armor_change.emit(armor)

	if remaining_damage > 0:
		health -= remaining_damage

	if armor_block_damage:
		pass
		#AudioManager.play_armor_block()
	else:
		pass
		#AudioManager.play_player_hurt()


func _on_wave_finished() -> void:
	armor = 0