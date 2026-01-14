class_name EventOptionData
extends RefCounted

var text: String
var data: Variant

func _init(p_text: String, p_data: Variant) -> void:
    text = p_text
    data = p_data