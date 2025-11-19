class_name TowerStatsPanel extends Control

@onready var damage_label: Label = $VBoxContainer/HBoxContainer/DamageLabel
@onready var attack_speed_label: Label = $VBoxContainer/HBoxContainer2/AttackSpeedLabel
@onready var range_label: Label = $VBoxContainer/HBoxContainer3/RangeLabel
@onready var critic_label: Label = $VBoxContainer/HBoxContainer4/CriticLabel
	
func update_stats(tower_stats: TowerStats) -> void:
	damage_label.text = str(int(tower_stats.damage))
	attack_speed_label.text = str(tower_stats.attack_speed)
	range_label.text = str(int(tower_stats.attack_range))
	critic_label.text = str(int(tower_stats.critic_chance))+"%"
