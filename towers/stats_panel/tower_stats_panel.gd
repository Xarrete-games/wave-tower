class_name TowerStatsPanel extends CanvasLayer

@onready var damage_label: Label = $PanelContainer/VBoxContainer/HBoxContainer/VBoxContainer/DamageLabel
@onready var attack_speed_label: Label = $PanelContainer/VBoxContainer/HBoxContainer2/VBoxContainer/AttackSpeedLabel
@onready var range_label: Label = $PanelContainer/VBoxContainer/HBoxContainer3/VBoxContainer/RangeLabel
@onready var critic_label: Label = $PanelContainer/VBoxContainer/HBoxContainer4/VBoxContainer2/CriticLabel

func update_stats(tower_stats: TowerStats) -> void:
	damage_label.text = str(tower_stats.damage)
	attack_speed_label.text = str(tower_stats.attack_speed)
	range_label.text = str(int(tower_stats.attack_range))
	critic_label.text = str(int(tower_stats.critic_chance))+"%"
