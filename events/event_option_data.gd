class_name EventOptionData
extends RefCounted

var text: String
var data: Variant
var disabled: bool = false

func _init(p_text: String, p_data: Variant, p_disabled: bool = false) -> void:
    text = p_text
    data = p_data
    disabled = p_disabled