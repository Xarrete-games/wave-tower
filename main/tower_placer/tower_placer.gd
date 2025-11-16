class_name TowerPlacer extends Node2D

signal tower_placed(tower_type: Tower.TowerType, amount: int)

var level_tile_map: LevelTileMap
# parent for all towers
var towers_container: Node2D
var _is_placing = false
var _current_tower_instance: Tower = null
var _is_valid_placement = false

var towers_placed = {
	Tower.TowerType.RED: 0,
	Tower.TowerType.BLUE: 0,
	Tower.TowerType.GREEN: 0
}

@export var towers_menu: TowersMenu

func reset_towers_count() -> void:
	_update_tower_count(Tower.TowerType.RED, 0)
	_update_tower_count(Tower.TowerType.GREEN, 0)
	_update_tower_count(Tower.TowerType.BLUE, 0)

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
	if not _is_placing:
		return
	# place tower
	if (event is InputEventMouseButton and 
		event.button_index == MOUSE_BUTTON_LEFT and 
		event.pressed and _is_valid_placement):
			_place_tower()
	# calcel
	elif event.is_action("exit"):
		_current_tower_instance.queue_free()
		_current_tower_instance = null
		_is_placing = false

# should be called each time a level is created
func update_nodes_from_current_level(current_level: Level) -> void:
	level_tile_map = current_level.get_node("LevelTileMap")
	towers_container = current_level.get_node("TowersContainer")

func _update_tower_count(tower_type: Tower.TowerType, value: int) -> void:
	towers_placed[tower_type] = value
	tower_placed.emit(tower_type, value)
	Price.tower_placed(tower_type, value)

func _place_tower() -> void:
	var tile_pos = level_tile_map.get_mouse_tile_pos()
	level_tile_map.set_tile_occupied(tile_pos)
	var tower_price = Price.get_price(_current_tower_instance.type)
	Score.substract_gold(tower_price)
	_is_placing = false
	
	var tower_type = _current_tower_instance.type
	_update_tower_count(tower_type, towers_placed[tower_type] + 1)
	
	_current_tower_instance.enable()
	_current_tower_instance = null

func _on_tower_selected(tower_scene: PackedScene) -> void:
	if _is_placing:
		return
	
	_current_tower_instance = tower_scene.instantiate()
	towers_container.add_child(_current_tower_instance)
	_is_placing = true
	
	
