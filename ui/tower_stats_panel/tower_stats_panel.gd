@tool
class_name TowerStatsPanel extends Control

# stats
@export var damage_label: Label
@export var attack_speed_label: Label
@export var range_label: Label
@export var critic_label: Label

# exp
@export var level_label: Label
@export var current_exp_label: Label
@export var required_exp_label: Label

var curre_tower: Tower

func _ready() -> void:
	visible = false
	TowerPlacementManager.tower_selected.connect(_on_tower_selected)

func _on_tower_selected(tower: Tower) -> void:
	if tower == null:
		visible = false
		return
	
	visible = true
	curre_tower = tower
	var stats = tower.stats
	var exp_data = tower.exp_data	
	update_stats(stats)
	update_exp_data(exp_data)

func update_stats(tower_stats: TowerStats) -> void:
	damage_label.text = str(tower_stats.damage)
	attack_speed_label.text = str(tower_stats.attack_speed)
	range_label.text = str(int(tower_stats.attack_range))
	critic_label.text = str(int(tower_stats.critic_chance))+"%"
	
func update_exp_data(exp_data: TowerExpData) -> void:
	level_label.text = str(exp_data.level)
	current_exp_label.text = str(exp_data.current_exp)
	required_exp_label.text = str(exp_data.exp_for_next_level)

func _on_sell_button_pressed() -> void:
	curre_tower.sell()
