class_name TowersCounters extends Control

const RED_TOWER = preload("uid://d36t7geqp1sh")
const BLUE_TOWER = preload("uid://cwyyp2r266blt")
const GREEN_TOWER = preload("uid://oj5ilwusjvuo")

#COUNTERS
@onready var red_tower_count: TowerCounterPanel = $RedTowerCount
@onready var green_tower_count: TowerCounterPanel = $GreenTowerCount
@onready var blue_tower_count: TowerCounterPanel = $BlueTowerCount

func _ready():
	RunContext.tower_count.tower_count_change.connect(_on_tower_count_change)
	
func _on_tower_count_change(tower_type: Tower.Type, amount: int) -> void:
	match tower_type:
		Tower.Type.RED: red_tower_count.count = amount
		Tower.Type.GREEN: green_tower_count.count = amount
		Tower.Type.BLUE: blue_tower_count.count = amount
