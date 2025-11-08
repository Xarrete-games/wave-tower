class_name Game extends Node2D

@export var levels: Array[PackedScene]
@export var current_level_number: int = 1

#current level parent
@onready var level: Node2D = $Level
@onready var tower_placer: TowerPlacer = $TowerPlacer
@onready var enemy_generator: EnemyGenerator = $EnemyGenerator

var _current_level: Level

func _ready():
	_load_level(current_level_number)
	
func _load_level(level_number: int) -> void:
	_current_level = levels[level_number - 1].instantiate()
	current_level_number = level_number
	level.add_child(_current_level)
	tower_placer.update_nodes_from_current_level(_current_level)
	enemy_generator.load_level_nodes(_current_level)
	enemy_generator.init()
	
