class_name Game extends Node2D

@export var levels: Array[PackedScene]
@export var current_level_number: int = 1

var _current_level: Level

#current level parent
@onready var level_container: Node2D = $LevelContainer
@onready var tower_placer: TowerPlacer = $TowerPlacer
@onready var enemy_generator: EnemyGenerator = $EnemyGenerator

func _ready():
	_load_level(current_level_number)
	
func _load_level(level_number: int) -> void:
	_current_level = levels[level_number - 1].instantiate()
	current_level_number = level_number
	level_container.add_child(_current_level)
	tower_placer.update_nodes_from_current_level(_current_level)
	enemy_generator.load_level_nodes(_current_level)
	enemy_generator.init()
	
