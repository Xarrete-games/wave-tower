class_name TowersMenu extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

#BUTTONS
@onready var red_tower_button: TowerButton = $TowersButtons/RedTowerButton
@onready var green_tower_button: TowerButton = $TowersButtons/GreenTowerButton
@onready var blue_tower_button: TowerButton = $TowersButtons/BlueTowerButton

func _ready():	
	await RunContext.initialized
	RunContext.towers_price.tower_price_change.connect(_on_tower_price_change)
	RunContext.economy.available_free_towers_change.connect(_on_available_free_towers_change)
	red_tower_button.price = RunContext.towers_price.get_price(Tower.Type.RED)
	green_tower_button.price = RunContext.towers_price.get_price(Tower.Type.GREEN)
	blue_tower_button.price = RunContext.towers_price.get_price(Tower.Type.BLUE)

func _on_red_tower_button_pressed(tower_scene: PackedScene) -> void:
	ClickEvents.tower_button_pressed.emit(tower_scene)

func _on_green_tower_button_pressed(tower_scene: PackedScene) -> void:
	ClickEvents.tower_button_pressed.emit(tower_scene)

func _on_blue_tower_button_pressed(tower_scene: PackedScene) -> void:
	ClickEvents.tower_button_pressed.emit(tower_scene)

func _on_tower_price_change(tower_type: Tower.Type, price: int) -> void:
	if RunContext.economy.available_free_towers > 0:
		return
	match tower_type:
		Tower.Type.RED: red_tower_button.price = price
		Tower.Type.GREEN: green_tower_button.price = price
		Tower.Type.BLUE: blue_tower_button.price = price

func _on_available_free_towers_change(available_free_towers: int) -> void:
	if available_free_towers > 0:
		red_tower_button.price = 0
		green_tower_button.price = 0
		blue_tower_button.price = 0
	else:
		red_tower_button.price = RunContext.towers_price.get_price(Tower.Type.RED)
		green_tower_button.price = RunContext.towers_price.get_price(Tower.Type.GREEN)
		blue_tower_button.price = RunContext.towers_price.get_price(Tower.Type.BLUE)