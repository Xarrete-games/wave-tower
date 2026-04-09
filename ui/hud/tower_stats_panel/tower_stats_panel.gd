class_name TowerStatsPanel extends Control


const TOWER_BUTTON = preload("uid://o248nju46k2n")

var current_tower: Tower
var button_in_hover: TowerButton = null

# definition
@onready var name_label: Label = %NameLabel
@onready var id_label: Label = %IdLabel
# stats
@onready var damage_stat: TowerStatUi = %DamageStatUi
@onready var attack_speed_stat: TowerStatUi = %AttkSpeedStatUi
@onready var range_stat: TowerStatUi = %RangeStatUi
# exp
@onready var level_container: Control = %LevelContainer
@onready var level_label: Label = %LevelLabel

# upgrades
@onready var upgrade_button_container: Control = %UpgradeButtonContainer
@onready var upgrade_tower_price: GoldPrice = %UpgradeTowerPrice
# tageting
@onready var targeting_mode_selector: OptionButton = %TargetingModeSelector

@onready var tower_hint_panel: TowerButtonHint = %TowerHintPanel

func _ready() -> void:
	visible = false
	tower_hint_panel.visible = false
	_hide_upgrade_options()
	ClickEvents.tower_selected.connect(_on_tower_selected)

func _on_tower_selected(tower: Tower) -> void:
	if tower == null:
		current_tower = null
		if ActionManager.CurrentAction == ActionManager.ActionState.TowerSelected:
			ActionManager.EndAction()
		return
	
	var tageting_modes: Array[Tower.TargetingMode] = [Tower.TargetingMode.FIRST_IN_PROGRESS]
	Hooks.on_get_targeting_modes(tageting_modes)
	_update_targeting_modes(tageting_modes)
	targeting_mode_selector.select(tower.targeting_mode)
	ActionManager.StartAction(ActionManager.ActionState.TowerSelected, func(): visible = false)
	visible = true

	var stats = tower.stats
	var exp_data = tower.exp_data
	update_stats(stats)
	update_exp_data(exp_data)

	name_label.text = tower.data.display_name
	id_label.text = tower.id
	current_tower = tower

	level_label.text = str(tower.level)
	if tower.is_max_level():
		# dont show updgrades
		_hide_upgrade_options()
	else:
		upgrade_tower_price.price = tower.data.upgrade_price
		upgrade_button_container.visible = true

	
func update_stats(tower_stats) -> void:
	if tower_stats == null:
		return
	damage_stat.set_value(tower_stats.damage)
	attack_speed_stat.set_value(tower_stats.attack_speed)
	range_stat.set_value(tower_stats.attack_range)
	
func update_exp_data(exp_data: TowerExpData) -> void:
	if exp_data == null:
		return
	level_label.text = str(exp_data.level)
	
func _update_targeting_modes(modes: Array[Tower.TargetingMode]) -> void:
	targeting_mode_selector.clear()
	for mode in modes:
		var mode_name = Tower.targeting_mode_to_string(mode)
		targeting_mode_selector.add_item(mode_name, mode)

func _on_targeting_mode_selector_item_selected(index: Tower.TargetingMode) -> void:
	current_tower.targeting_mode = index

func _on_remove_button_pressed() -> void:
	ClickEvents.tower_remove_pressed.emit(current_tower)

func _on_upgrade_button_pressed() -> void:
	if RunContext.economy.gold < current_tower.data.upgrade_price:
		return

	current_tower.upgrade()

func _hide_upgrade_options() -> void:
	upgrade_button_container.visible = false


func _on_tower_button_pressed(tower_data, price: int) -> void:
	ClickEvents.tower_upgrade_pressed.emit(current_tower, tower_data, price)

func _on_tower_button_hover(tower_button: TowerButton) -> void:
	button_in_hover = tower_button
	tower_hint_panel.set_stats(button_in_hover.configuration)
	# Position the hint above the button
	var rect: Rect2 = tower_button.get_global_rect()	
	tower_hint_panel.global_position = Vector2(
		rect.position.x + rect.size.x * 0.5 - tower_hint_panel.size.x * 0.5,
		rect.position.y - tower_hint_panel.size.y - 30
	)
	tower_hint_panel.visible = true

func _on_tower_button_unhover(tower_button: TowerButton) -> void:
	if button_in_hover == tower_button:
		button_in_hover = null
		tower_hint_panel.visible = false

func _on_upgrade_button_xarreta_mouse_entered() -> void:
	damage_stat.show_upgrade_value(current_tower.data.stats_on_level.damage)
	attack_speed_stat.show_upgrade_value(current_tower.data.stats_on_level.attack_speed)
	range_stat.show_upgrade_value(current_tower.data.stats_on_level.attack_range)

func _on_upgrade_button_xarreta_mouse_exited() -> void:
	damage_stat.hide_upgrade_value()
	attack_speed_stat.hide_upgrade_value()
	range_stat.hide_upgrade_value()
