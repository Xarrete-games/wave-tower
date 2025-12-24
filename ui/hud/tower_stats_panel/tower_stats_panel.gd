@tool
class_name TowerStatsPanel extends Control

# stats
@export var damage_stat: TowerStatUi
@export var attack_speed_stat: TowerStatUi
@export var range_stat: TowerStatUi

# exp
@export var level_label: Label
@export var current_exp_label: Label
@export var required_exp_label: Label


@export var upgrade_button_container: Control
@export var targeting_mode_selector: OptionButton


var current_tower: Tower

func _ready() -> void:
	visible = false
	upgrade_button_container.visible = false
	ClickEvents.tower_selected.connect(_on_tower_selected)
	await RunContext.initialized
	RunContext.towers_upgrades.targeting_modes_change.connect(_update_targeting_modes)
	_update_targeting_modes(RunContext.towers_upgrades.targeting_modes)

func _on_tower_selected(tower: Tower) -> void:
	if tower == null:
		visible = false
		return
	
	targeting_mode_selector.select(tower.targeting_mode)
	visible = true
	current_tower = tower
	var stats = tower.stats
	var exp_data = tower.exp_data
	update_stats(stats)
	update_exp_data(exp_data)

	if tower is BasicTower:
		upgrade_button_container.visible = true
	else:
		upgrade_button_container.visible = false

func update_stats(tower_stats: TowerStats) -> void:
	damage_stat.set_value(tower_stats.damage)
	attack_speed_stat.set_value(tower_stats.attack_speed)
	range_stat.set_value(tower_stats.attack_range)
	
func update_exp_data(exp_data: TowerExpData) -> void:
	level_label.text = str(exp_data.level)
	current_exp_label.text = str(exp_data.current_exp)
	required_exp_label.text = str(exp_data.exp_for_next_level)

func _update_targeting_modes(modes: Array[Tower.TargetingMode]) -> void:
	targeting_mode_selector.clear()
	for mode in modes:
		var mode_name = RunContext.towers_upgrades.targeting_mode_to_string(mode)
		targeting_mode_selector.add_item(mode_name, mode)

func _on_targeting_mode_selector_item_selected(index: Tower.TargetingMode) -> void:
	current_tower.targeting_mode = index

func _on_remove_button_pressed() -> void:
	ClickEvents.tower_sold_pressed.emit(current_tower)
