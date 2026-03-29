class_name Relic extends AbstractItem

var data: RelicData
var disabled: bool = false

var id: String:
	get:
		return data.id

func _init(p_data: RelicData) -> void:
	data = p_data

func apply_effect() -> void:
	pass

func remove_effect() -> void:
	pass

func on_obtain() -> void:
	pass

func on_remove() -> void:
	pass