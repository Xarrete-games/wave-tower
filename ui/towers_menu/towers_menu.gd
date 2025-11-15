class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

signal tower_selected(tower_scene: PackedScene)

@export var tower_placer: TowerPlacer

func _ready():
	pass
	
func _on_red_tower_button_pressed() -> void:
	if Score.gold >= Price.TowerBuild.RED:
		tower_selected.emit(RED_TOWER)

func _on_blue_tower_button_pressed() -> void:
	if Score.gold >= Price.TowerBuild.BLUE:
		tower_selected.emit(BLUE_TOWER)

func _on_green_tower_button_pressed() -> void:
	if Score.gold >= Price.TowerBuild.GREEN:
		tower_selected.emit(GREEN_TOWER)

func _on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	pass
