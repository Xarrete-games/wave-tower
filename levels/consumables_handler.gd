class_name ConsumablesHandler extends Node

# for 48x48 cursor icons
const CENTER_CURSOR_OFFSET: Vector2 = Vector2(24, 24)

var _current_consumable: ConsumableTargeteable = null
var _is_valid_target: bool = false
var _current_target: Variant

@onready var level_tile_map: LevelTileMap = %'LevelTileMap'

func _ready() -> void:
	RunContext.consumables.consumable_clicked.connect(_on_consumable_clicked)
	GameState.state_change.connect(_on_game_state_changed)
	ClickEvents.tower_hovered.connect(_on_tower_hovered)
	ClickEvents.tower_unhovered.connect(_on_tower_unhovered)

func _process(_delta: float) -> void:
	if not _current_consumable:
		return
	
	if _current_consumable.targeting_type == ConsumableTargeteable.TargetType.BLOCKED_TILE:
		_handle_blocked_tile_placement()
		
func _input(event: InputEvent) -> void:
	if not _current_consumable:
		return

	if Utils.is_left_click_event(event) and _is_valid_target:
		_use_consumable()
	elif event.is_action("exit"):
		cancel_current_consumable()

func _handle_blocked_tile_placement() -> void:
	if level_tile_map.is_mouse_on_block_tile():
		Input.set_custom_mouse_cursor(_current_consumable.cursor_icon_used, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_is_valid_target = true
		_current_target = level_tile_map
	else:
		Input.set_custom_mouse_cursor(_current_consumable.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_invalidate_target()

func _invalidate_target() -> void:
	_is_valid_target = false
	_current_target = null

func _on_tower_hovered(tower: Tower) -> void:
	if not _current_consumable or _current_consumable.targeting_type != ConsumableTargeteable.TargetType.TOWER:
		return
	
	Input.set_custom_mouse_cursor(_current_consumable.cursor_icon_used, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
	_is_valid_target = true
	_current_target = tower

func _on_tower_unhovered(tower: Tower) -> void:
	if not _current_consumable or _current_consumable.targeting_type != ConsumableTargeteable.TargetType.TOWER:
		return
	
	if tower == _current_target:
		Input.set_custom_mouse_cursor(_current_consumable.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_invalidate_target()

func _use_consumable() -> void:
	if not _current_consumable:
		return
	_current_consumable.use(_current_target)
	_current_consumable = null
	GameState.state = GameState.STATE.IN_GAME
	Input.set_custom_mouse_cursor(null)


func cancel_current_consumable() -> void:
	_current_consumable = null
	await get_tree().process_frame 
	await get_tree().process_frame
	if GameState.is_using_item():
		GameState.state = GameState.STATE.IN_GAME
	Input.set_custom_mouse_cursor(null)

func _on_consumable_clicked(consumable: Consumable) -> void:
	_current_consumable = consumable
	GameState.state = GameState.STATE.USING_ITEM
	Input.set_custom_mouse_cursor(_current_consumable.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)

func _on_game_state_changed(new_state: GameState.STATE) -> void:
	if new_state != GameState.STATE.USING_ITEM and _current_consumable:
		cancel_current_consumable()
