@abstract
class_name EventScript extends RefCounted

@abstract
func get_options() -> Array[EventOptionData]

@abstract
func handle_response(data: Variant) -> void