class_name TowerExpData extends RefCounted

var level: int = 1
var current_exp: int = 0
var exp_for_next_level: int = 0

func _init(new_level: int, new_current_exp: int, new_exp_for_next_level: int) -> void:
	level = new_level
	current_exp = new_current_exp
	exp_for_next_level = new_exp_for_next_level
