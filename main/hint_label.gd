class_name HintLabel extends Label

const INITIAL_HINT = "[WASD] Move Camera"

var is_first_value: bool = true


var wait_first_hint: bool = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	text = INITIAL_HINT
	GameState.state_change.connect(_on_state_change)
	await get_tree().create_timer(10, false).timeout
	if text == INITIAL_HINT:
		wait_first_hint = false
		text = ""

func _on_state_change(state: GameState.STATE) -> void:
	if state != GameState.STATE.PLACING_TOWER and state != GameState.STATE.USING_ITEM:
		if not wait_first_hint:
			text = ""
	else:
		wait_first_hint = false
		text = "[ESC] Cancel"
