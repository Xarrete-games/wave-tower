class_name TowerPlacer extends Node2D

var level_tile_map: LevelTileMap
# parent for all towers
var towers_container: Node2D
var _is_placing = false:
	set(value):
		_is_placing = value
		TowerPlacementManager.is_placing = value
var _current_tower_instance: Tower = null
var _is_valid_placement = false

@export var towers_menu: TowersMenu

func _ready():
	_is_placing = false
	towers_menu.tower_selected.connect(_on_tower_selected)
	EnemyManager.wave_finished.connect(_cancel_tower)
	EnemyManager.last_wave_finished.connect(_cancel_tower)

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
	if not _is_placing:
		return
	# place tower
	if (event is InputEventMouseButton and 
		event.button_index == MOUSE_BUTTON_LEFT and 
		event.pressed and _is_valid_placement):
			_place_tower()
	# calcel
	elif event.is_action("exit"):
		_cancel_tower()
		

# should be called each time a level is created
func update_nodes_from_current_level(current_level: Level) -> void:
	level_tile_map = current_level.get_node("LevelTileMap")
	towers_container = current_level.get_node("TowersContainer")

func _place_tower() -> void:
	var tile_pos = level_tile_map.get_mouse_tile_pos()
	level_tile_map.set_tile_occupied(tile_pos)
	var tower_price = Price.get_price(_current_tower_instance.type)
	Score.substract_gold(tower_price)
	_is_placing = false
	
	TowerPlacementManager.tower_added(_current_tower_instance)
	
	_current_tower_instance.enable()
	_current_tower_instance.tile_pos = tile_pos
	_current_tower_instance = null

func _cancel_tower() -> void:
	if _current_tower_instance:
		_current_tower_instance.queue_free()
		_current_tower_instance = null
		await get_tree().process_frame 
		await get_tree().process_frame
		_is_placing = false

func _on_tower_selected(tower_scene: PackedScene) -> void:
	if _is_placing:
		return
	
	_current_tower_instance = tower_scene.instantiate()
	towers_container.add_child(_current_tower_instance)
	TowerPlacementManager.clear_tower_selected()
	_is_placing = true
