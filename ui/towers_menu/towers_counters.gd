class_name TowersCounters extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

#COUNTERS
@onready var red_tower_count: TowerCount = $RedTowerCount
@onready var green_tower_count: TowerCount = $GreenTowerCount
@onready var blue_tower_count: TowerCount = $BlueTowerCount

func _ready():
	TowerPlacementManager.tower_count_change.connect(_on_tower_count_change)
	
func _on_tower_count_change(
	tower_type: Tower.TowerType, 
	amount: int, 
	_event: TowerPlacementManager.TowerEvent) -> void:
	match tower_type:
		Tower.TowerType.RED: red_tower_count.count = amount
		Tower.TowerType.GREEN: green_tower_count.count = amount
		Tower.TowerType.BLUE: blue_tower_count.count = amount
