class_name TowerStatsPanel extends CanvasLayer

# stats
@onready var damage_label: Label = $PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/DamageLabel
@onready var attack_speed_label: Label = $PanelContainer/VBoxContainer/HBoxContainer2/VBoxContainer/AttackSpeedLabel
@onready var range_label: Label = $PanelContainer/VBoxContainer/HBoxContainer3/VBoxContainer/RangeLabel
@onready var critic_label: Label = $PanelContainer/VBoxContainer/HBoxContainer4/VBoxContainer2/CriticLabel
@onready var panel_container: PanelContainer = $PanelContainer

# exp
@onready var level_label: Label = $PanelContainer/VBoxContainer/ExpDataContainer/LevelContainer/LevelLabel
@onready var current_exp_label: Label = $PanelContainer/VBoxContainer/ExpDataContainer/ExpContainer/CurrentExpLabel
@onready var required_exp_label: Label = $PanelContainer/VBoxContainer/ExpDataContainer/ExpContainer/RequiredExpLabel


func set_panel_position(new_position: Vector2) -> void:
	panel_container.position = new_position

func update_stats(tower_stats: TowerStats, exp_data: TowerExpData) -> void:
	damage_label.text = str(tower_stats.damage)
	attack_speed_label.text = str(tower_stats.attack_speed)
	range_label.text = str(int(tower_stats.attack_range))
	critic_label.text = str(int(tower_stats.critic_chance))+"%"
	
	update_exp_data(exp_data)

func update_exp_data(exp_data: TowerExpData) -> void:
	level_label.text = str(exp_data.level)
	current_exp_label.text = str(exp_data.current_exp)
	required_exp_label.text = str(exp_data.exp_for_next_level)
