class_name ConsumablesHandler extends Node

var _current_consumable: Consumable = null
var _is_valid_placement: bool = false

@onready var level_tile_map: LevelTileMap = %'LevelTileMap'
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	RunContext.consumables.consumable_clicked.connect(_on_consumable_clicked)
	GameState.state_change.connect(_on_game_state_changed)


func _process(_delta: float) -> void:
	if not _current_consumable:
		return
	
	if level_tile_map.is_mouse_on_block_tile():
		Input.set_custom_mouse_cursor(_current_consumable.cursor_icon_used, Input.CURSOR_ARROW, Vector2(24, 24))
		_is_valid_placement = true
	else:
		Input.set_custom_mouse_cursor(_current_consumable.cursor_icon, Input.CURSOR_ARROW, Vector2(24, 24))
		_is_valid_placement = false


func _input(event: InputEvent) -> void:
	if not _current_consumable:
		return
	# place tower
	if Utils.is_left_click_event(event) and _is_valid_placement:
		_use_consumable()
	elif event.is_action("exit"):
		cancel_current_consumable()

func _use_consumable() -> void:
	if not _current_consumable:
		return
	_current_consumable.use(level_tile_map)
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

func _on_game_state_changed(new_state: GameState.STATE) -> void:
	if new_state != GameState.STATE.USING_ITEM and _current_consumable:
		cancel_current_consumable()
