class_name TowerPlacer extends Node2D

@onready var level_tile_map: LevelTileMap = $'../LevelTileMap'
@onready var visual: Node2D = $'../Visual'

var _is_placing = false:
	set(value):
		_is_placing = value
		if _is_placing:
			GameState.state = GameState.STATE.PLACING_TOWER
		else:
			if GameState.is_placing_tower():
				GameState.state = GameState.STATE.IN_GAME

var _current_tower_instance: Tower = null
var _is_valid_placement = false

func _ready():
	_is_placing = false
	ClickEvents.tower_build_button_pressed.connect(_on_tower_button_pressed)
	ClickEvents.tower_upgrade_pressed.connect(_on_tower_upgrade_pressed)
	RunContext.progress.current_wave_finished.connect(_cancel_tower)
	RunContext.progress.last_wave_finished.connect(_cancel_tower)

func _process(_delta: float) -> void:
	if not _is_placing or not is_instance_valid(_current_tower_instance):
		return
	
	if level_tile_map.is_mouse_on_buildeable_tile():
		_is_valid_placement = true
		_current_tower_instance.normal_color()
		_current_tower_instance.global_position = level_tile_map.get_current_tile_pos()
	else:
		_is_valid_placement = false
		_current_tower_instance.phantom_mode()
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
		
func _place_tower() -> void:
	if not _current_tower_instance:
		return
	# check gold
	var tower_price = _current_tower_instance.build_price
	if not _has_enought_gold(tower_price):
		_cancel_tower()
		return
	_handle_costs(tower_price)
	# place tower
	var tile_pos = level_tile_map.get_mouse_tile_pos()
	level_tile_map.set_tile_occupied(tile_pos)
	
	_is_placing = false
	
	_current_tower_instance.enable()
	_current_tower_instance.tile_pos = tile_pos

	RunContext.towers_count.tower_added(_current_tower_instance)

	_current_tower_instance = null

func _has_enought_gold(tower_price: int) -> bool:
	if RunContext.economy.available_free_towers > 0:
		return true
	elif RunContext.economy.gold >= tower_price:
		return true
	return false

func _handle_costs(tower_price: int) -> void:
	if RunContext.economy.available_free_towers > 0:
		RunContext.economy.available_free_towers -= 1
	else:
		RunContext.economy.gold -= tower_price

func _cancel_tower() -> void:
	if _current_tower_instance:
		_current_tower_instance.queue_free()
		_current_tower_instance = null
		await get_tree().process_frame 
		await get_tree().process_frame
		_is_placing = false

func _on_tower_button_pressed(tower_configuration: TowerConfigurationWithInstance, price: int) -> void:
	if _is_placing:
		return
	
	_current_tower_instance = tower_configuration.get_instance()
	_current_tower_instance.build_price = price
	visual.add_child(_current_tower_instance)
	_is_placing = true

func _on_tower_upgrade_pressed(current_tower: Tower, new_tower_conf: TowerConfigurationWithInstance, price: int) -> void:
	var new_tower = new_tower_conf.get_instance()
	new_tower.build_price = price
	RunContext.economy.gold -= price
	visual.add_child(new_tower)
	new_tower.global_position = current_tower.global_position
	new_tower.tile_pos = current_tower.tile_pos
	new_tower.enable()
	current_tower.queue_free()
	ClickEvents.tower_selected.emit(new_tower)
