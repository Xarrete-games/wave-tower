class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")


signal tower_selected(tower_scene: PackedScene)

#BUTTONS
@onready var red_tower_button: TowerButton = $RedTowerButton
@onready var green_tower_button: TowerButton = $GreenTowerButton
@onready var blue_tower_button: TowerButton = $BlueTowerButton
@onready var gold_price: GoldPrice = $MarginContainer/VBoxContainer/GoldPrice

func _ready():
	Price.tower_price_change.connect(_on_tower_price_change)
	RewardsManager.show_rewards_price_change.connect(
		func(value): gold_price.price = value)
	red_tower_button.price = Price.get_price(Tower.TowerType.RED)
	green_tower_button.price = Price.get_price(Tower.TowerType.GREEN)
	blue_tower_button.price = Price.get_price(Tower.TowerType.BLUE)
	gold_price.price = RewardsManager.show_rewards_price

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


func _on_rewards_ui_buton_pressed() -> void:
	if RewardsManager.show_rewards_price <= Score.gold:
		RewardsManager.show_rewards_price += 50
		RewardsManager.show_rewards_ui()
