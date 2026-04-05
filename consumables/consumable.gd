class_name Consumable extends RefCounted

signal used(consumable: Consumable)
signal clicked(consumable: Consumable)

enum Type { OTHER, POTION }

var data: ConsumableData

func _init(p_data: ConsumableData) -> void:
	data = p_data

func get_source() -> Source:
	return Source.new(Source.SourceType.CONSUMABLE, data.id, self)