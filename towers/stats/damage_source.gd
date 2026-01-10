class_name DamageSource extends RefCounted

enum Type {
	TOWER,
	DEBUFF
}

var type: Type
var id: String
var type_id: String

func _init(p_type: Type, p_id: String, p_type_id: String) -> void:
	type = p_type
	id = p_id
	type_id = p_type_id
