class_name TowerSelector extends Node

var current_tower_selected: Tower

func _ready() -> void:
	# initialize to no tower selected
	clear_tower_selected()
	ClickEvents.tower_selected.connect(_on_tower_selected)
	ClickEvents.tower_button_pressed.connect(_on_tower_button_pressed)
	ClickEvents.tower_remove_pressed.connect(_on_tower_remove_pressed)
	RunContext.progress.current_wave_finished.connect(clear_tower_selected)

func _input(event: InputEvent) -> void:
	if Utils.is_right_click_event(event):
		clear_tower_selected()

func _unhandled_input(event: InputEvent) -> void:
	if Utils.is_left_click_event(event):
		clear_tower_selected()
		get_viewport().set_input_as_handled()

func clear_tower_selected() -> void:
	if current_tower_selected:
		current_tower_selected.stats_change.disconnect(_on_stats_change)

	current_tower_selected = null
	ClickEvents.tower_selected.emit(current_tower_selected)

func _on_tower_selected(tower: Tower) -> void:
	if tower == null:
		return

	if not current_tower_selected:
		current_tower_selected = tower
		current_tower_selected.stats_change.connect(_on_stats_change)
	elif current_tower_selected != tower:
		current_tower_selected.stats_change.disconnect(_on_stats_change)
		current_tower_selected = tower
		current_tower_selected.stats_change.connect(_on_stats_change)

func _on_tower_button_pressed(_tower_configuration: TowerConfigurationWithInstance) -> void:
	clear_tower_selected()

func _on_tower_remove_pressed(_tower: Tower) -> void:
	clear_tower_selected()

func _on_stats_change(tower: Tower) -> void:
	if tower == current_tower_selected:
		ClickEvents.tower_selected.emit(tower)
