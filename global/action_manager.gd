extends Node


enum ActionState { None, PlacingTower, UsingItem, TowerSelected }

var CurrentAction: ActionState = ActionState.None
var _onActionCancel: Callable = Callable()

func StartAction(state: ActionState, cancel_callback: Callable = Callable()) -> void:
	if CurrentAction != ActionState.None:
		EndAction()
	CurrentAction = state
	_onActionCancel = cancel_callback

func EndAction() -> void:
	var callback := _onActionCancel
	_onActionCancel = Callable()
	if callback.is_valid():
		callback.call()
	CurrentAction = ActionState.None

func IsActionActive() -> bool:
	return CurrentAction != ActionState.None
