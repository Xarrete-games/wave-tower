class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

signal tower_selected(tower_scene: PackedScene)

@export var tower_placer: TowerPlacer

#COUNTERS
@onready var red_tower_count: TowerCount = $TowersCount/RedTowerCount
@onready var green_tower_count: TowerCount = $TowersCount/GreenTowerCount
@onready var blue_tower_count: TowerCount = $TowersCount/BlueTowerCount

#BUTTONS
@onready var red_tower_button: TowerButton = $CenterContainer/HBoxContainer/RedTowerButton
@onready var green_tower_button: TowerButton = $CenterContainer/HBoxContainer/GreenTowerButton
@onready var blue_tower_button: TowerButton = $CenterContainer/HBoxContainer/BlueTowerButton

func _ready():
	tower_placer.tower_placed.connect(_on_tower_placed)
	Price.tower_price_change.connect(_on_tower_price_change)
	
func _on_tower_placed(tower_type: Tower.TowerType, amount: int) -> void:
	match tower_type:
		Tower.TowerType.RED: red_tower_count.count = amount
		Tower.TowerType.GREEN: green_tower_count.count = amount
		Tower.TowerType.BLUE: blue_tower_count.count = amount

func _on_red_tower_button_pressed(tower_scene: PackedScene) -> void:
	tower_selected.emit(tower_scene)

func _on_green_tower_button_pressed(tower_scene: PackedScene) -> void:
	tower_selected.emit(tower_scene)

func _on_blue_tower_button_pressed(tower_scene: PackedScene) -> void:
	tower_selected.emit(tower_scene)

func _on_tower_price_change(tower_type: Tower.TowerType, price: int) -> void:
	match tower_type:
		Tower.TowerType.RED: red_tower_button.price = price
		Tower.TowerType.GREEN: green_tower_button.price = price
		Tower.TowerType.BLUE: blue_tower_button.price = price
