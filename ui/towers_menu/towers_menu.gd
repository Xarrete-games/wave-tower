class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")

signal tower_selected(tower_scene: PackedScene)

func _ready():
	pass
	
func _on_red_tower_button_pressed() -> void:
	if Score.gold >= Price.TowerBuild.RED:
		tower_selected.emit(RED_TOWER)
