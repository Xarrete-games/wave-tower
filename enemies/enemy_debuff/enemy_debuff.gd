class_name EnemyDebuff extends RefCounted

enum DebuffType { FROST }

var type: DebuffType 
var value: float
var duration: float

func _init(new_type: DebuffType, new_value: float, new_duration: float) -> void:
	type = new_type
	value = new_value
	duration = new_duration
	
