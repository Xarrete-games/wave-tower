#GameState
extends Node

signal state_change(state: STATE)
signal speed_change(value: float)

enum STATE { ON_MAIN_MENU, IN_GAME, PLACING_TOWER }

var speed: float:
	get(): return _speed
	set(value):
		_speed = value
		Engine.time_scale = value
		speed_change.emit(value)
var _speed: float = 1.0

var state: STATE:
	set(value):
		state = value
		state_change.emit(state)

func _ready() -> void:
	ClickEvents.speed_button_pressed.connect(_button_speed_pressed)

func is_on_main_menu() -> bool:
	return state == STATE.ON_MAIN_MENU

func is_is_in_game() -> bool:
	return state == STATE.IN_GAME

func is_placing_tower() -> bool:
	return state == STATE.PLACING_TOWER

func reset_run() -> void:

	EnemyDebuffManager.reset()
	speed = 1.0

func _button_speed_pressed() -> void:
	if speed == 1.0:
		speed = 2.0
	elif speed == 2.0:
		speed = 3.0
	elif speed == 3.0:
		speed = 1.0
