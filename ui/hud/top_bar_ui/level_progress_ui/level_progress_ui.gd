class_name LevelProgressUI extends HBoxContainer

const LEVEL_PROGRESS_SLOT = preload("uid://dddlo6uv4ct15")
const SKULL_ICON = preload("uid://35i1uisepolx")
const QUESTION_ICON = preload("uid://ew6iu1r5ngoe")
const RELIC_ICON = preload("uid://dlix83276rvep")
const SHOP_ICON = preload("uid://0jpdo5boqutp")

const WAVES_WITH_EVENTS = [8]
const WAVES_WITH_SHOPS = [4]
const WAVES_WITH_RELICS = [2,6]
const WAVES_WITH_BOSS = [10]

func _ready() -> void:
	_clear()
	RunContext.progress.current_wave_changed.connect(_on_wave_init)
	RunContext.progress.current_level_changed.connect(_on_new_level)
	ClickEvents.reset_game_button_pressed.connect(func () -> void:
		_clear()
	)

func _clear() -> void:
	for child in get_children():
		child.queue_free()
	_build()

func _build() -> void:
	for index in range(10):
		var slot: LevelProgressSlot = LEVEL_PROGRESS_SLOT.instantiate()
		add_child(slot)
		
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
	if new_value % 10 == 0:
		_clear()
	var value = new_value
	while value > 10:
		value -= 10

	var slot: LevelProgressSlot = get_child(value - 1)
	slot.fill()

func _on_new_level(_total_waves: int) -> void:
	_clear()

func _on_mouse_entered() -> void:
	ClickEvents.level_progess_hovered.emit()

func _on_mouse_exited() -> void:
	ClickEvents.level_progess_unhovered.emit()
