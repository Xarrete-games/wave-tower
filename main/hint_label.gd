class_name HintLabel extends Label

const INITIAL_HINT = "[WASD] Move Camera"
const GAME_STATE_IN_GAME := 1

var _wait_first_hint: bool = true

func _ready() -> void:
	text = INITIAL_HINT
	GameState.state_change.connect(_on_state_change)
	await get_tree().create_timer(10, false).timeout
	if text == INITIAL_HINT:
		_wait_first_hint = false
		text = ""

func _on_state_change(state: int) -> void:
	if state == GAME_STATE_IN_GAME and not _wait_first_hint:
		text = ""
