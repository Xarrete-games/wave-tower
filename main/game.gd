class_name Game extends Node2D

@export var levels: Array[PackedScene]
@export var current_level_number: int = 1

var _current_level: Level

#current level parent
@onready var level_container: Node2D = $LevelContainer
@onready var tower_placer: TowerPlacer = $TowerPlacer
@onready var enemy_generator: EnemyGenerator = $EnemyGenerator
@onready var next_level_menu: NextLevelMenu = $UILayer/NextLevelMenu

func _ready():
	_hide_next_level_menu()
	_load_level(current_level_number)
		
func _load_level(level_number: int) -> void:
	_current_level = levels[level_number - 1].instantiate()
	current_level_number = level_number
	level_container.add_child(_current_level)
	tower_placer.update_nodes_from_current_level(_current_level)
	enemy_generator.load_level_nodes(_current_level)
	enemy_generator.init_wave()

func _hide_next_level_menu() -> void:
	next_level_menu.visible = false
	next_level_menu.mouse_filter = Control.MOUSE_FILTER_IGNORE
	next_level_menu.set_process(false)
	
func _show_next_level_menu() -> void:
	next_level_menu.visible = true
	next_level_menu.mouse_filter = Control.MOUSE_FILTER_STOP
	next_level_menu.set_process(true)
	

func _on_enemy_generator_last_wave_done() -> void:
	_show_next_level_menu()

func _on_next_level_menu_next_leve_button_pressed() -> void:
	_hide_next_level_menu()
	current_level_number += 1
	_current_level.queue_free()
	_load_level(current_level_number)
	
