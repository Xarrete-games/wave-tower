extends Node


enum ActionState { NONE, PLACING_TOWER, USING_ITEM, TOWER_SELECTED }

var current_action: ActionState = ActionState.NONE
var on_action_cancel: Callable

func start_action(state: ActionState, cancel_callback: Callable = func(): pass) -> void:
	if current_action != ActionState.NONE:
		end_action()
	current_action = state
	on_action_cancel = cancel_callback

func end_action() -> void:
	if on_action_cancel != null:
		on_action_cancel.call()
	current_action = ActionState.NONE

func is_action_active() -> bool:
	return current_action != ActionState.NONE
