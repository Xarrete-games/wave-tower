extends HBoxContainer

const LEVEL_PROGRESS_SLOT = preload("uid://dddlo6uv4ct15")
const SKULL_ICON = preload("uid://35i1uisepolx")
const QUESTION_ICON = preload("uid://ew6iu1r5ngoe")

func _ready() -> void:
	_clear()
	EnemyManager.wave_init.connect(_on_wave_init)
	EnemyManager.new_level_loaded.connect(_on_new_level)

func _clear() -> void:
	for child in get_children():
		child.queue_free()
	_build()

func _build() -> void:
	for index in range(10):
		var slot: LevelProgressSlot = LEVEL_PROGRESS_SLOT.instantiate()
		add_child(slot)
		if index == 2 or index == 5 or index == 8:
			slot.set_icon(QUESTION_ICON)	
		if index == 9:
			slot.set_icon(SKULL_ICON)

func _on_wave_init(new_value: int) -> void:
	var slot: LevelProgressSlot = get_child(new_value - 1)
	slot.fill()

func _on_new_level(_total_waves: int) -> void:
	_clear()
