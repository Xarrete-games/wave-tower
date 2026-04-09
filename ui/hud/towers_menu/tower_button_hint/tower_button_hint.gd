class_name TowerButtonHint extends Control

@onready var name_label: RichTextLabel = %NameLabel
@onready var description_label: Label = %DescriptionLabel
@onready var damage_stat: TowerStatUi = %DamageStatUi
@onready var attack_speed_stat: TowerStatUi = %AttkSpeedStatUi
@onready var range_stat: TowerStatUi = %RangeStatUi

func set_stats(configuration) -> void:
	name_label.text = "[u]" + configuration.display_name + "[/u]"
	description_label.text = configuration.description
	damage_stat.set_value(configuration.base_damage)
	attack_speed_stat.set_value(configuration.base_attack_speed)
	range_stat.set_value(configuration.base_attack_range)
