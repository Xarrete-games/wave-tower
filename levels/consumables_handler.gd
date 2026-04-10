class_name ConsumablesHandler extends Node

# for 48x48 cursor icons
const CENTER_CURSOR_OFFSET: Vector2 = Vector2(24, 24)
const DEFAULT_CURSOR: Texture2D = preload("res://assets/images/icons/mouse_02.png")
const TARGET_BLOCKED_TILE := 0
const TARGET_TOWER := 1
const ACTION_USING_ITEM := 2
const GAME_STATE_IN_GAME := 1

var _current_consumable = null
var _is_valid_target: bool = false
var _current_target: Variant

@export var composite_tile_map: CompositeTileMap

func _ready() -> void:
	RunContext.consumables_manager.consumable_clicked.connect(_on_consumable_clicked)
	GameState.state_change.connect(_on_game_state_changed)
	ClickEvents.connect("tower_hovered", _on_tower_hovered)
	ClickEvents.connect("tower_unhovered", _on_tower_unhovered)

func _process(_delta: float) -> void:
	if not _current_consumable:
		return
	
	if _current_consumable.data.targeting_type == TARGET_BLOCKED_TILE:
		_handle_blocked_tile_placement()
		
func _input(event: InputEvent) -> void:
	if not _current_consumable:
		return

	if UIUtils.is_left_click_event(event) and _is_valid_target:
		_use_consumable()

func _handle_blocked_tile_placement() -> void:
	if composite_tile_map.is_mouse_on_block_tile():
		Input.set_custom_mouse_cursor(_current_consumable.data.cursor_icon_used, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_is_valid_target = true
		_current_target = composite_tile_map
	else:
		Input.set_custom_mouse_cursor(_current_consumable.data.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_invalidate_target()

func _invalidate_target() -> void:
	_is_valid_target = false
	_current_target = null

func _on_tower_hovered(tower: Tower) -> void:
	if not _current_consumable or _current_consumable.data.targeting_type != TARGET_TOWER:
		return
	
	Input.set_custom_mouse_cursor(_current_consumable.data.cursor_icon_used, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
	_is_valid_target = true
	_current_target = tower

func _on_tower_unhovered(tower: Tower) -> void:
	if not _current_consumable or _current_consumable.data.targeting_type != TARGET_TOWER:
		return
	
	if tower == _current_target:
		Input.set_custom_mouse_cursor(_current_consumable.data.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
		_invalidate_target()

func _use_consumable() -> void:
	if not _current_consumable:
		return
	_current_consumable.use(_current_target)
	_current_consumable = null
	Input.set_custom_mouse_cursor(DEFAULT_CURSOR)
	
	ActionManager.EndAction()

func _cancel_consumable() -> void:
	_current_consumable = null
	Input.set_custom_mouse_cursor(DEFAULT_CURSOR)

func _on_consumable_clicked(consumable) -> void:
	if consumable == null:
		return
	if not consumable.has_method("requires_target") or not consumable.requires_target():
		return

	_current_consumable = consumable
	Input.set_custom_mouse_cursor(_current_consumable.data.cursor_icon, Input.CURSOR_ARROW, CENTER_CURSOR_OFFSET)
	
	ActionManager.StartAction(ACTION_USING_ITEM, _cancel_consumable)

func _on_game_state_changed(new_state: int) -> void:
	if new_state != GAME_STATE_IN_GAME and _current_consumable:
		ActionManager.EndAction()
