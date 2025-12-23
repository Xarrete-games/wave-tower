class_name TowerButtonHint extends Control

@export var name_label: RichTextLabel
@export var description_label: Label
@export var damage_label: Label
@export var attack_speed_label: Label
@export var range_label: Label

func set_stats(tower_stats: TowerConfiguration) -> void:
	name_label.text = "[u]" + tower_stats.definition.display_name + "[/u]"
	description_label.text = tower_stats.definition.description
	damage_label.text = str(tower_stats.base_damage)
	attack_speed_label.text = str(tower_stats.base_attack_speed)
	range_label.text = str(tower_stats.base_attack_range)
