class_name LevelProgressUI extends Control

const LEVEL_PROGRESS_SLOT = preload("uid://dddlo6uv4ct15")
const SKULL_ICON = preload("uid://drk0ngia6t0aq")
const QUESTION_ICON = preload("uid://ba8nsfyfw75as")
const RELIC_ICON = preload("uid://rvo816wmsjvc")
const SHOP_ICON = preload("uid://cerww538cqrba")

const WAVES_WITH_EVENTS = [8]
const WAVES_WITH_SHOPS = [4]
const WAVES_WITH_RELICS = [2,6]
const WAVES_WITH_BOSS = [10]

@export var slots_container: Control

func _ready() -> void:
	_clear()
	RunContext.progress.current_wave_changed.connect(_on_wave_init)
	ClickEvents.reset_game_button_pressed.connect(func () -> void:
		_clear()
	)

func _clear() -> void:
	for child in slots_container.get_children():
		slots_container.remove_child(child)
		child.queue_free()
	_build()

func _build() -> void:
	for index in range(10):
		var slot: LevelProgressSlot = LEVEL_PROGRESS_SLOT.instantiate()
		slots_container.add_child(slot)
		
		var wave_number = index + 1

		if	wave_number in WAVES_WITH_EVENTS:
			slot.set_icon(QUESTION_ICON)
		elif wave_number in WAVES_WITH_SHOPS:
			slot.set_icon(SHOP_ICON)
		elif wave_number in WAVES_WITH_RELICS:
			slot.set_icon(RELIC_ICON)
		elif wave_number in WAVES_WITH_BOSS:
			slot.set_icon(SKULL_ICON)
		else:
			slot.set_icon(null)

func _on_wave_init(new_value: int) -> void:
	if new_value % 10 == 1:
		_clear()

	var value: int = ((new_value - 1) % 10) + 1

	var slot: LevelProgressSlot = slots_container.get_child(value - 1)
	slot.fill()


func _on_mouse_entered() -> void:
	ClickEvents.level_progess_hovered.emit()

func _on_mouse_exited() -> void:
	ClickEvents.level_progess_unhovered.emit()
