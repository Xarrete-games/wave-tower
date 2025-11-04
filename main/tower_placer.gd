class_name TowerPlacer extends Node2D

@onready var towers_menu: TowersMenu = $"../GameUI/TowersMenu"
@onready var level_tile_map: LevelTileMap = $"../LevelTileMap"


var _is_placing = false
var _current_tower_instance: Tower = null

# state
var _is_valid_placement = false

func _ready():
	towers_menu.tower_selected.connect(_on_tower_selected)

func _process(_delta: float) -> void:
	if not _is_placing or not is_instance_valid(_current_tower_instance):
		return
	
	if level_tile_map.is_mouse_on_buildeable_tile():
		_is_valid_placement = true
		_current_tower_instance.global_position = level_tile_map.get_current_tile_pos()
	else:
		_is_valid_placement = false
		_current_tower_instance.global_position = get_global_mouse_position()

func _input(event: InputEvent) -> void:
	if _is_placing and event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT and event.pressed:

		if _is_valid_placement:
			_place_tower()

func _place_tower() -> void:
	var tile_pos = level_tile_map.get_mouse_tile_pos()
	level_tile_map.set_tile_occupied(tile_pos)
	Score.substract_gold(_current_tower_instance.build_price)
	_is_placing = false
	_current_tower_instance = null

func _on_tower_selected(tower_scene: PackedScene) -> void:
	if _is_placing:
		return
	
	_current_tower_instance = tower_scene.instantiate()
	add_child(_current_tower_instance)
	_is_placing = true
	
	
