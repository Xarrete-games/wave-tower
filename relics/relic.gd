class_name Relic extends AbstractModel

signal changed(relic: Relic)

var data: RelicData
var disabled: bool = false:
	set(value):
		disabled = value
		changed.emit(self)

var counter: int = 0:
	set(value):
		counter = value
		changed.emit(self)

var id: String:
	get:
		return data.id

func _init(p_data: RelicData) -> void:
	data = p_data

func get_source() -> Source:
	return Source.new(Source.SourceType.RELIC, id, self)

func on_obtain() -> void:
	pass

func on_remove() -> void:
	pass
