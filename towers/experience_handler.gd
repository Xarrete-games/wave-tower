class_name ExprienceHandler extends Node

signal exp_data_change(new_exp_data: TowerExpData)
signal level_up(new_level: int)

const MAX_LEVEL = 10
const EXP_BY_HIT = 1

var dic_exp_required: Dictionary[int, int] = {
	1: 40,
	2: 50,
	3: 50,
	4: 50,
	5: 50,
	6: 60,
	7: 60,
	8: 60,
	9: 80,
}
var tower: Tower
var level: int = 1:
	set(value):
		level = value
		exp_data.level = value
		exp_data_change.emit(exp_data)
		level_up.emit(value)
		
var current_exp: float = 0:
	set(value):
		current_exp = value
		exp_data.current_exp = int(value)
		exp_data_change.emit(exp_data)
		 
var exp_for_next_level = dic_exp_required[1]:
	set(value):
		exp_for_next_level = value
		exp_data.exp_for_next_level = value
		exp_data_change.emit(exp_data)
var exp_data: TowerExpData = TowerExpData.new(level, int(current_exp), exp_for_next_level)
var attack_speed 


func _ready() -> void:
	tower = get_parent()
	tower.attack_speed_change.connect(_on_attack_speed_change)
	tower.attack_fired.connect(_on_attack_fired)
	_update_requited_exp()

func _on_attack_fired() -> void:
	current_exp += (EXP_BY_HIT * attack_speed)
	if current_exp >= exp_for_next_level:
		_level_up()

func _on_attack_speed_change(value: float) -> void:
	attack_speed = value

func _update_requited_exp() -> void:
	exp_for_next_level = dic_exp_required[level]
	current_exp = 0
	
func _level_up() -> void:
	level += 1
	_update_requited_exp()

# SINCRONICE DATA WITH TOWER
func _on_level_up(new_level: int) -> void:
	exp_data.level = new_level
	exp_data_change.emit(exp_data)
	
func _current_exp_change(new_current_exp: int) -> void:
	exp_data.current_exp = new_current_exp
	exp_data_change.emit(exp_data)
	
func _exp_for_next_level_change(new_exp_for_next_level: int) -> void:
	exp_data.exp_for_next_level = new_exp_for_next_level
	exp_data_change.emit(exp_data)
	
