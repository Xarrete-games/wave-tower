class_name Consumable extends RefCounted

signal used(consumable: Consumable)
signal clicked(consumable: Consumable)

enum Type { OTHER, POTION }

var data

func _init(p_data) -> void:
	data = p_data

func requires_target() -> bool:
	return false

func get_source() -> Source:
	return Source.new(Source.SourceType.CONSUMABLE, data.id, self)
